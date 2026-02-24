using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Invoices;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;
    private readonly IInvoiceCalculationService _calc;
    private readonly ICurrentUserContext _currentUser;

    public InvoiceService(AppDbContext db, IInvoiceCalculationService calc, ICurrentUserContext currentUser)
    {
        _db = db;
        _calc = calc;
        _currentUser = currentUser;
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

    public async Task<PagedResult<InvoiceListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, InvoiceStatus? status, Guid? customerId, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.InvoiceDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.InvoiceDate <= dateTo.Value.Date);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
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
                CustomerTitle = x.CustomerTitle,
                GrandTotal = x.GrandTotal,
                Currency = x.Currency
            })
            .ToListAsync(ct);
        return new PagedResult<InvoiceListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var inv = await ScopeQuery()
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return inv == null ? null : MapToDto(inv);
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

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = request.InvoiceDate.Date,
            Status = InvoiceStatus.Draft,
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
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
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

        return MapToDto(invoice);
    }

    public async Task<InvoiceDto?> UpdateAsync(Guid id, UpdateInvoiceRequest request, byte[]? rowVersion, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice == null) return null;

        if (invoice.Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Paid invoice cannot be updated.");
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cancelled invoice cannot be modified.");

        if (rowVersion != null && (invoice.RowVersion == null || !invoice.RowVersion.SequenceEqual(rowVersion)))
            throw new InvalidOperationException("Invoice was modified by another user. Please refresh and try again.");

        var userId = _currentUser.UserId;

        invoice.InvoiceDate = request.InvoiceDate.Date;
        invoice.CustomerTitle = request.CustomerTitle;
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
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
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

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> SetStatusAsync(Guid id, InvoiceStatus status, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice == null) return false;
        if (invoice.Status == InvoiceStatus.Cancelled) return false;
        if (invoice.Status == InvoiceStatus.Paid && status != InvoiceStatus.Paid) return false;
        invoice.Status = status;
        invoice.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<string> GetNextInvoiceNumberAsync(Guid userId, int year, CancellationToken ct)
    {
        var last = await _db.Invoices
            .Where(x => x.UserId == userId && x.InvoiceDate.Year == year)
            .OrderByDescending(x => x.InvoiceNumber)
            .Select(x => x.InvoiceNumber)
            .FirstOrDefaultAsync(ct);
        if (string.IsNullOrEmpty(last))
            return $"{year}-0001";
        var parts = last.Split('-');
        if (parts.Length != 2 || !int.TryParse(parts[1], out var num))
            return $"{year}-0001";
        return $"{year}-{(num + 1):D4}";
    }

    private static InvoiceDto MapToDto(Invoice inv) => new()
    {
        Id = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        InvoiceDate = inv.InvoiceDate,
        Status = inv.Status,
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
        Items = inv.Items.OrderBy(x => x.SortOrder).Select(x => new InvoiceItemDto
        {
            Id = x.Id,
            Description = x.Description,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            VatRate = x.VatRate,
            LineTotalExclVat = x.LineTotalExclVat,
            LineVatAmount = x.LineVatAmount,
            LineTotalInclVat = x.LineTotalInclVat,
            SortOrder = x.SortOrder
        }).ToList()
    };
}
