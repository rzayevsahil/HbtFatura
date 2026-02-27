using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Invoices;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Helpers;

namespace HbtFatura.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;
    private readonly IInvoiceCalculationService _calc;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public InvoiceService(AppDbContext db, IInvoiceCalculationService calc, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _calc = calc;
        _currentUser = currentUser;
        _log = log;
    }

    private IQueryable<Invoice> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.Invoices.Where(i => i.User != null && i.User.FirmId == firmIdFilter.Value);
            return _db.Invoices.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin)
            return _db.Invoices.Where(i => i.User != null && i.User.FirmId == _currentUser.FirmId);
        return _db.Invoices.Where(i => i.UserId == _currentUser.UserId);
    }

    public async Task<PagedResult<InvoiceListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, InvoiceStatus? status, InvoiceType? invoiceType, Guid? customerId, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.InvoiceDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.InvoiceDate <= dateTo.Value.Date);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (invoiceType.HasValue) query = query.Where(x => x.InvoiceType == invoiceType.Value);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.InvoiceDate)
            .ThenByDescending(x => x.InvoiceNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InvoiceListDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceDate = x.InvoiceDate,
                Status = x.Status,
                InvoiceType = x.InvoiceType,
                CustomerTitle = x.CustomerTitle,
                GrandTotal = x.GrandTotal,
                Currency = x.Currency,
                IsGibSent = x.IsGibSent,
                SourceType = x.SourceType
            })
            .ToListAsync(ct);
        return new PagedResult<InvoiceListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var inv = await ScopeQuery()
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (inv == null) return null;
        var dto = MapToDto(inv);
        if (inv.SourceType == ReferenceType.Irsaliye && inv.SourceId.HasValue)
        {
            var dn = await _db.DeliveryNotes.Select(d => new { d.Id, d.DeliveryNumber }).FirstOrDefaultAsync(x => x.Id == inv.SourceId.Value, ct);
            if (dn != null) dto.SourceNumber = dn.DeliveryNumber;
        }
        else if (inv.SourceType == ReferenceType.Siparis && inv.SourceId.HasValue)
        {
            var order = await _db.Orders.Select(o => new { o.Id, o.OrderNumber }).FirstOrDefaultAsync(x => x.Id == inv.SourceId.Value, ct);
            if (order != null) dto.SourceNumber = order.OrderNumber;
        }
        return dto;
    }

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var invoiceNumber = await GetNextInvoiceNumberAsync(userId, request.InvoiceDate.Year, ct);

        string customerTitle = request.CustomerTitle;
        string? customerTaxNumber = request.CustomerTaxNumber;
        string? customerAddress = request.CustomerAddress;
        string? customerPhone = request.CustomerPhone;
        string? customerEmail = request.CustomerEmail;
        Guid? customerId = request.CustomerId;

        if (request.CustomerId.HasValue)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId && (c.UserId == userId || (_currentUser.IsFirmAdmin && c.User != null && c.User.FirmId == _currentUser.FirmId)), ct);
            if (customer != null)
            {
                customerTitle = customer.Title;
                customerTaxNumber = customer.TaxNumber;
                customerAddress = customer.Address;
                customerPhone = customer.Phone;
                customerEmail = customer.Email;
            }
        }

        if (string.IsNullOrWhiteSpace(customerTaxNumber)) throw new ArgumentException("Müşterinin Vergi/TC numarası eksik. Lütfen cari kartını güncelleyin veya manuel girin.");
        if (string.IsNullOrWhiteSpace(customerAddress)) throw new ArgumentException("Müşterinin adresi eksik. Lütfen cari kartını güncelleyin veya manuel girin.");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTimeHelper.NormalizeForStorage(request.InvoiceDate),
            Status = InvoiceStatus.Draft,
            InvoiceType = request.InvoiceType,
            CustomerId = customerId,
            CustomerTitle = customerTitle,
            CustomerTaxNumber = customerTaxNumber,
            CustomerAddress = customerAddress,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            Currency = request.Currency,
            ExchangeRate = request.ExchangeRate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var sortOrder = 0;
        foreach (var item in request.Items)
        {
            var entity = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                ProductId = item.ProductId,
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                DiscountPercent = item.DiscountPercent,
                SortOrder = sortOrder++
            };
            _calc.CalculateItemTotals(entity);
            invoice.Items.Add(entity);
        }
        _calc.CalculateInvoiceTotals(invoice);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }

        invoice = (await ScopeQuery().Include(x => x.Items.OrderBy(i => i.SortOrder)).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == invoice.Id, ct))!;
        return MapToDto(invoice);
    }

    public async Task<InvoiceDto?> CreateFromDeliveryNoteAsync(Guid deliveryNoteId, CancellationToken ct = default)
    {
        var dn = await _db.DeliveryNotes
            .Include(x => x.User)
            .Include(x => x.Customer)
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(x => x.Id == deliveryNoteId, ct);
        if (dn == null) return null;
        if (!_currentUser.IsSuperAdmin && !(_currentUser.IsFirmAdmin && dn.User?.FirmId == _currentUser.FirmId) && dn.UserId != _currentUser.UserId)
            return null;
        if (dn.Status != DeliveryNoteStatus.Onaylandi)
            throw new InvalidOperationException("Sadece onaylanmış irsaliyelerden fatura oluşturulabilir.");

        var userId = _currentUser.UserId;
        var invoiceNumber = await GetNextInvoiceNumberAsync(userId, dn.DeliveryDate.Year, ct);

        var customerTitle = dn.Customer?.Title ?? string.Empty;
        var customerTaxNumber = dn.Customer?.TaxNumber;
        var customerAddress = dn.Customer?.Address;
        var customerPhone = dn.Customer?.Phone;
        var customerEmail = dn.Customer?.Email;

        if (string.IsNullOrWhiteSpace(customerTaxNumber)) throw new ArgumentException("İrsaliyedeki müşterinin Vergi/TC numarası eksik. Fatura oluşturulamaz.");
        if (string.IsNullOrWhiteSpace(customerAddress)) throw new ArgumentException("İrsaliyedeki müşterinin adresi eksik. Fatura oluşturulamaz.");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = dn.DeliveryDate,
            Status = InvoiceStatus.Draft,
            InvoiceType = dn.DeliveryType,
            CustomerId = dn.CustomerId,
            CustomerTitle = customerTitle,
            CustomerTaxNumber = customerTaxNumber,
            CustomerAddress = customerAddress,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            Currency = "TRY",
            ExchangeRate = 1,
            SourceType = ReferenceType.Irsaliye,
            SourceId = dn.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var sortOrder = 0;
        foreach (var item in dn.Items)
        {
            var entity = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                ProductId = item.ProductId,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                DiscountPercent = 0,
                SortOrder = sortOrder++
            };
            _calc.CalculateItemTotals(entity);
            invoice.Items.Add(entity);
        }
        _calc.CalculateInvoiceTotals(invoice);

        _db.Invoices.Add(invoice);
        dn.InvoiceId = invoice.Id;
        dn.Status = DeliveryNoteStatus.Faturalandi;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(invoice.Id, ct);
    }

    public async Task<InvoiceDto?> UpdateAsync(Guid id, UpdateInvoiceRequest request, byte[]? rowVersion, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery().Include(x => x.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice == null) return null;

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Paid invoice cannot be updated.");
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cancelled invoice cannot be modified.");

        if (rowVersion != null && (invoice.RowVersion == null || !invoice.RowVersion.SequenceEqual(rowVersion)))
            throw new InvalidOperationException("Invoice was modified by another user. Please refresh and try again.");

        var userId = _currentUser.UserId;

        invoice.InvoiceDate = DateTimeHelper.NormalizeForStorage(request.InvoiceDate);
        invoice.InvoiceType = request.InvoiceType;
        invoice.CustomerId = request.CustomerId;
        invoice.CustomerTitle = request.CustomerTitle;
        
        if (string.IsNullOrWhiteSpace(request.CustomerTaxNumber)) throw new ArgumentException("Vergi/TC numarası boş olamaz.");
        if (string.IsNullOrWhiteSpace(request.CustomerAddress)) throw new ArgumentException("Adres boş olamaz.");

        invoice.CustomerTaxNumber = request.CustomerTaxNumber;
        invoice.CustomerAddress = request.CustomerAddress;
        invoice.CustomerPhone = request.CustomerPhone;
        invoice.CustomerEmail = request.CustomerEmail;
        invoice.Currency = request.Currency;
        invoice.ExchangeRate = request.ExchangeRate;
        invoice.UpdatedAt = DateTime.UtcNow;
        invoice.UpdatedBy = userId;

        if (request.CustomerId.HasValue)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId && (c.UserId == invoice.UserId || (_currentUser.IsFirmAdmin && c.User != null && c.User.FirmId == _currentUser.FirmId)), ct);
            if (customer != null)
            {
                invoice.CustomerId = customer.Id;
                invoice.CustomerTitle = customer.Title;
                invoice.CustomerTaxNumber = customer.TaxNumber;
                invoice.CustomerAddress = customer.Address;
                invoice.CustomerPhone = customer.Phone;
                invoice.CustomerEmail = customer.Email;
            }
        }
        else
        {
            invoice.CustomerId = null;
        }

        _db.InvoiceItems.RemoveRange(invoice.Items);
        var sortOrder = 0;
        foreach (var item in request.Items)
        {
            var entity = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                ProductId = item.ProductId,
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                DiscountPercent = item.DiscountPercent,
                SortOrder = sortOrder++
            };
            _calc.CalculateItemTotals(entity);
            invoice.Items.Add(entity);
        }
        _calc.CalculateInvoiceTotals(invoice);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            throw new InvalidOperationException("Invoice was modified by another user. Please refresh and try again.");
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }

        invoice = (await ScopeQuery().Include(x => x.Items.OrderBy(i => i.SortOrder)).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == invoice.Id, ct))!;
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> SendToGibAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice == null) return false;
        
        if (invoice.IsGibSent) return true;

        var ok = await SetStatusAsync(id, InvoiceStatus.Issued, ct);
        if (!ok) return false;

        invoice.IsGibSent = true;
        await _db.SaveChangesAsync(ct);

        await _log.LogAsync($"Fatura GİB'e gönderildi: {invoice.InvoiceNumber}", "SendToGib", "Invoice", "Info", $"Id: {invoice.Id}");

        return true;
    }

    public async Task<bool> SetStatusAsync(Guid id, InvoiceStatus status, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice == null) return false;
        if (invoice.Status == InvoiceStatus.Cancelled) return false;
        if (invoice.Status == InvoiceStatus.Paid && status != InvoiceStatus.Paid) return false;

        if ((status == InvoiceStatus.Issued || status == InvoiceStatus.Paid) && invoice.CustomerId.HasValue)
        {
            var alreadyCreated = await _db.AccountTransactions.AnyAsync(t => t.ReferenceType == ReferenceType.Fatura && t.ReferenceId == id, ct);
            if (!alreadyCreated)
            {
                var cariType = invoice.InvoiceType == InvoiceType.Alis ? AccountTransactionType.Borc : AccountTransactionType.Alacak;
                _db.AccountTransactions.Add(new AccountTransaction
                {
                    Id = Guid.NewGuid(),
                    CustomerId = invoice.CustomerId.Value,
                    UserId = invoice.UserId,
                    Date = invoice.InvoiceDate,
                    Type = cariType,
                    Amount = invoice.GrandTotal,
                    Currency = invoice.Currency,
                    Description = $"Fatura {invoice.InvoiceNumber}",
                    ReferenceType = ReferenceType.Fatura,
                    ReferenceId = id,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

if (status == InvoiceStatus.Issued || status == InvoiceStatus.Paid)
            {
                var stockAlreadyCreated = await _db.StockMovements.AnyAsync(m => m.ReferenceType == ReferenceType.Fatura && m.ReferenceId == id, ct);
                if (!stockAlreadyCreated)
                {
                    var stockDirection = invoice.InvoiceType == InvoiceType.Alis ? StockMovementType.Giris : StockMovementType.Cikis;
                    foreach (var item in invoice.Items.Where(i => i.ProductId.HasValue))
                    {
                        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId!.Value, ct);
                        if (product != null)
                        {
                            if (stockDirection == StockMovementType.Cikis && item.Quantity > product.StockQuantity)
                                throw new InvalidOperationException($"'{product.Name}' ürünü için yetersiz stok! Mevcut: {product.StockQuantity}");

                            if (stockDirection == StockMovementType.Giris)
                                product.StockQuantity += item.Quantity;
                            else
                                product.StockQuantity -= item.Quantity;
                        }

                        _db.StockMovements.Add(new StockMovement
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId!.Value,
                            Date = invoice.InvoiceDate,
                            Type = stockDirection,
                        Quantity = item.Quantity,
                        ReferenceType = ReferenceType.Fatura,
                        ReferenceId = id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        invoice.Status = status;
        invoice.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<string> GetNextInvoiceNumberAsync(Guid userId, int year, CancellationToken ct)
    {
        var prefix = "FTR";
        var yearStr = year.ToString();
        var last = await _db.Invoices
            .Where(x => x.UserId == userId && x.InvoiceNumber.StartsWith(prefix + yearStr))
            .OrderByDescending(x => x.InvoiceNumber)
            .Select(x => x.InvoiceNumber)
            .FirstOrDefaultAsync(ct);
            
        if (string.IsNullOrEmpty(last) || last.Length < 16)
            return $"{prefix}{yearStr}000000001";
            
        var seqStr = last.Substring(7); // 3 (FTR) + 4 (YEAR) = 7
        if (long.TryParse(seqStr, out var num))
        {
            return $"{prefix}{yearStr}{(num + 1):D9}";
        }
        return $"{prefix}{yearStr}000000001";
    }

    private static InvoiceDto MapToDto(Invoice inv) => new()
    {
        Id = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        InvoiceDate = inv.InvoiceDate,
        Status = inv.Status,
        InvoiceType = inv.InvoiceType,
        CustomerId = inv.CustomerId,
        CustomerTitle = inv.CustomerTitle,
        CustomerTaxNumber = inv.CustomerTaxNumber,
        CustomerAddress = inv.CustomerAddress,
        CustomerPhone = inv.CustomerPhone,
        CustomerEmail = inv.CustomerEmail,
        SubTotal = inv.SubTotal,
        TotalVat = inv.TotalVat,
        GrandTotal = inv.GrandTotal,
        Currency = inv.Currency,
        ExchangeRate = inv.ExchangeRate,
        SourceType = inv.SourceType,
        SourceId = inv.SourceId,
        IsGibSent = inv.IsGibSent,
        Items = inv.Items.OrderBy(x => x.SortOrder).Select(x => new InvoiceItemDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductCode = x.Product?.Code,
            Description = x.Description,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            VatRate = x.VatRate,
            DiscountPercent = x.DiscountPercent,
            LineTotalExclVat = x.LineTotalExclVat,
            LineVatAmount = x.LineVatAmount,
            LineTotalInclVat = x.LineTotalInclVat,
            SortOrder = x.SortOrder
        }).ToList()
    };
}
