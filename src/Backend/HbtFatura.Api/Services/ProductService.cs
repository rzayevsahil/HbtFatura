using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Product;
using HbtFatura.Api.Entities;
using InvoiceType = HbtFatura.Api.Entities.InvoiceType;

namespace HbtFatura.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public ProductService(AppDbContext db, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _currentUser = currentUser;
        _log = log;
    }

    private IQueryable<Product> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.Products.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.Products.AsQueryable();
        }
        if (_currentUser.FirmId.HasValue)
            return _db.Products.Where(x => x.FirmId == _currentUser.FirmId.Value);

        return _db.Products.Where(x => false);
    }

    private static string NormalizeProductCode(string code) => code.Trim().ToLowerInvariant();

    private async Task<bool> IsDuplicateProductCodeAsync(Guid firmId, string code, Guid? excludeProductId, CancellationToken ct)
    {
        var norm = NormalizeProductCode(code);
        return await _db.Products.AnyAsync(p =>
            p.FirmId == firmId &&
            (!excludeProductId.HasValue || p.Id != excludeProductId.Value) &&
            p.Code.ToLower() == norm, ct);
    }

    public async Task<bool> IsProductCodeTakenAsync(string code, Guid firmId, Guid? excludeProductId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        if (!_currentUser.IsSuperAdmin)
        {
            if (!_currentUser.FirmId.HasValue || _currentUser.FirmId.Value != firmId)
                throw new UnauthorizedAccessException();
        }
        return await IsDuplicateProductCodeAsync(firmId, code, excludeProductId, ct);
    }

    public async Task<PagedResult<ProductListDto>> GetPagedAsync(int page, int pageSize, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(s) ||
                x.Name.ToLower().Contains(s) ||
                (x.Barcode != null && x.Barcode.Contains(s)));
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductListDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Code = x.Code,
                Name = x.Name,
                Barcode = x.Barcode,
                Unit = x.Unit,
                StockQuantity = x.StockQuantity,
                UnitPrice = x.UnitPrice,
                Currency = x.Currency,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
        return new PagedResult<ProductListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        return new ProductDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Code = entity.Code,
            Name = entity.Name,
            Barcode = entity.Barcode,
            Unit = entity.Unit,
            StockQuantity = entity.StockQuantity,
            UnitPrice = entity.UnitPrice,
            Currency = entity.Currency,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        Guid firmId;
        if (_currentUser.IsSuperAdmin && request.FirmId.HasValue)
            firmId = request.FirmId.Value;
        else if (_currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        if (request.StockQuantity < 0) throw new ArgumentException("Başlangıç stok miktarı sıfırdan küçük olamaz.");

        if (await IsDuplicateProductCodeAsync(firmId, request.Code, null, ct))
            throw new ArgumentException("Bu ürün kodu bu firmada zaten kullanılıyor. Farklı bir kod girin.");

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            Barcode = request.Barcode?.Trim(),
            Unit = request.Unit?.Trim() ?? "Adet",
            StockQuantity = request.StockQuantity,
            UnitPrice = request.UnitPrice,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "TRY" : request.Currency.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _db.Products.Add(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ürün kartı oluşturuldu: {entity.Name} ({entity.Code})", "Create", "Product", "Info", $"Id: {entity.Id}");
        
        if (request.StockQuantity != 0)
        {
            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = entity.Id,
                Date = DateTime.UtcNow,
                Type = request.StockQuantity > 0 ? StockMovementType.Giris : StockMovementType.Cikis,
                Quantity = Math.Abs(request.StockQuantity),
                ReferenceType = ReferenceType.Manuel,
                CreatedAt = DateTime.UtcNow
            };
            _db.StockMovements.Add(movement);
            await _db.SaveChangesAsync(ct);
        }

        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;

        if (await IsDuplicateProductCodeAsync(entity.FirmId, request.Code, entity.Id, ct))
            throw new ArgumentException("Bu ürün kodu bu firmada zaten kullanılıyor. Farklı bir kod girin.");

        entity.Code = request.Code.Trim();
        entity.Name = request.Name.Trim();
        entity.Barcode = request.Barcode?.Trim();
        entity.Unit = request.Unit?.Trim() ?? "Adet";
        entity.UnitPrice = request.UnitPrice;
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? entity.Currency : request.Currency.Trim();
        
        if (request.StockQuantity < 0) throw new ArgumentException("Stok miktarı sıfırdan küçük olamaz.");

        var difference = request.StockQuantity - entity.StockQuantity;
        if (difference != 0)
        {
            entity.StockQuantity = request.StockQuantity;
            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = entity.Id,
                Date = DateTime.UtcNow,
                Type = difference > 0 ? StockMovementType.Giris : StockMovementType.Cikis,
                Quantity = Math.Abs(difference),
                ReferenceType = ReferenceType.Manuel,
                CreatedAt = DateTime.UtcNow
            };
            _db.StockMovements.Add(movement);
            await _db.SaveChangesAsync(ct);
        }
        
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ürün kartı güncellendi: {entity.Name} ({entity.Code})", "Update", "Product", "Info", $"Id: {entity.Id}");
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ürün kartı silindi: {entity.Name} ({entity.Code})", "Delete", "Product", "Warning", $"Id: {entity.Id}");
        return true;
    }

    public async Task<PagedResult<StockMovementDto>> GetMovementsAsync(Guid productId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == productId, ct);
        if (!hasAccess) return new PagedResult<StockMovementDto> { Items = new List<StockMovementDto>(), TotalCount = 0, Page = page, PageSize = pageSize };

        var query = _db.StockMovements.Where(m => m.ProductId == productId);
        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value;
            query = query.Where(m => m.Date >= from);
        }
        if (dateTo.HasValue)
        {
            var to = dateTo.Value;
            query = query.Where(m => m.Date <= to);
        }
        var total = await query.CountAsync(ct);

        var movements = await query
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Referanslara göre ilgili tablo kayıtlarını topla
        var invoiceIds = movements.Where(m => m.ReferenceType == ReferenceType.Fatura && m.ReferenceId.HasValue)
            .Select(m => m.ReferenceId!.Value).Distinct().ToList();
        var deliveryNoteIds = movements.Where(m => m.ReferenceType == ReferenceType.Irsaliye && m.ReferenceId.HasValue)
            .Select(m => m.ReferenceId!.Value).Distinct().ToList();
        var orderIds = movements.Where(m => m.ReferenceType == ReferenceType.Siparis && m.ReferenceId.HasValue)
            .Select(m => m.ReferenceId!.Value).Distinct().ToList();

        var invoices = await _db.Invoices
            .Where(i => invoiceIds.Contains(i.Id))
            .Select(i => new { i.Id, i.InvoiceNumber })
            .ToListAsync(ct);
        var invoiceDict = invoices.ToDictionary(i => i.Id);

        var deliveryNotes = await _db.DeliveryNotes
            .Where(d => deliveryNoteIds.Contains(d.Id))
            .Select(d => new { d.Id, d.DeliveryNumber, d.OrderId })
            .ToListAsync(ct);
        var dnDict = deliveryNotes.ToDictionary(d => d.Id);

        // İrsaliyeden oluşturulan faturaları da yakala (SourceType = Irsaliye, SourceId = dn.Id)
        var invoicesFromDn = await _db.Invoices
            .Where(i => i.SourceType == ReferenceType.Irsaliye && i.SourceId.HasValue && deliveryNoteIds.Contains(i.SourceId.Value))
            .Select(i => new { i.Id, i.InvoiceNumber, SourceId = i.SourceId!.Value })
            .ToListAsync(ct);
        var invoiceFromDnDict = invoicesFromDn.ToDictionary(i => i.SourceId);

        // Siparişler hem doğrudan stok hareketinden hem de irsaliyeden gelebilir
        var extraOrderIdsFromDn = deliveryNotes.Where(d => d.OrderId.HasValue).Select(d => d.OrderId!.Value);
        var allOrderIds = orderIds.Union(extraOrderIdsFromDn).Distinct().ToList();
        var orders = await _db.Orders
            .Where(o => allOrderIds.Contains(o.Id))
            .Select(o => new { o.Id, o.OrderNumber })
            .ToListAsync(ct);
        var orderDict = orders.ToDictionary(o => o.Id);

        var items = movements.Select(m =>
        {
            Guid? invoiceId = null;
            string? invoiceNumber = null;
            Guid? deliveryNoteId = null;
            string? deliveryNumber = null;
            Guid? orderId = null;
            string? orderNumber = null;

            if (m.ReferenceType == ReferenceType.Fatura && m.ReferenceId.HasValue && invoiceDict.TryGetValue(m.ReferenceId.Value, out var inv))
            {
                invoiceId = inv.Id;
                invoiceNumber = inv.InvoiceNumber;
            }
            else if (m.ReferenceType == ReferenceType.Irsaliye && m.ReferenceId.HasValue && dnDict.TryGetValue(m.ReferenceId.Value, out var dn))
            {
                deliveryNoteId = dn.Id;
                deliveryNumber = dn.DeliveryNumber;
                // İrsaliyeden kesilen faturayı bul
                if (invoiceFromDnDict.TryGetValue(dn.Id, out var invFromDn))
                {
                    invoiceId = invFromDn.Id;
                    invoiceNumber = invFromDn.InvoiceNumber;
                }
                if (dn.OrderId.HasValue && orderDict.TryGetValue(dn.OrderId.Value, out var ordFromDn))
                {
                    orderId = ordFromDn.Id;
                    orderNumber = ordFromDn.OrderNumber;
                }
            }
            else if (m.ReferenceType == ReferenceType.Siparis && m.ReferenceId.HasValue && orderDict.TryGetValue(m.ReferenceId.Value, out var ord))
            {
                orderId = ord.Id;
                orderNumber = ord.OrderNumber;
            }

            return new StockMovementDto
            {
                Id = m.Id,
                Date = m.Date,
                Type = m.Type,
                Quantity = m.Quantity,
                ReferenceType = m.ReferenceType,
                ReferenceId = m.ReferenceId,
                InvoiceId = invoiceId,
                InvoiceNumber = invoiceNumber,
                DeliveryNoteId = deliveryNoteId,
                DeliveryNumber = deliveryNumber,
                OrderId = orderId,
                OrderNumber = orderNumber,
                CreatedAt = m.CreatedAt
            };
        }).ToList();

        return new PagedResult<StockMovementDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<StockMovementDto> AddMovementAsync(Guid productId, CreateStockMovementRequest request, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == productId, ct);
        if (!hasAccess) throw new UnauthorizedAccessException("Product not found or access denied.");
        if (request.Quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (request.Type != StockMovementType.Giris && request.Type != StockMovementType.Cikis)
            throw new ArgumentException("Type must be Giris (1) or Cikis (2).");

        var entityStock = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Date = request.Date,
            Type = request.Type,
            Quantity = request.Quantity,
            ReferenceType = ReferenceType.Manuel,
            CreatedAt = DateTime.UtcNow
        };
        _db.StockMovements.Add(entityStock);
        
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == productId, ct);
        if (entity == null) throw new Exception("Product not found.");

        if (request.Type == StockMovementType.Cikis && request.Quantity > entity.StockQuantity)
            throw new InvalidOperationException("Yetersiz stok! Mevcut stoktan fazla çıkış yapamazsınız.");

        if (request.Type == StockMovementType.Giris)
            entity.StockQuantity += request.Quantity;
        else if (request.Type == StockMovementType.Cikis)
            entity.StockQuantity -= request.Quantity;


        await _db.SaveChangesAsync(ct);
        var typeLabel = request.Type == StockMovementType.Giris ? "Giriş" : "Çıkış";
        await _log.LogAsync($"Manuel stok hareketi: {entity.Code} — {typeLabel} {request.Quantity}", "StockMovement", "Product", "Info", $"ProductId: {productId}, HareketId: {entityStock.Id}");
        return new StockMovementDto
        {
            Id = entityStock.Id,
            Date = entityStock.Date,
            Type = entityStock.Type,
            Quantity = entityStock.Quantity,
            ReferenceType = entityStock.ReferenceType,
            ReferenceId = entityStock.ReferenceId,
            CreatedAt = entityStock.CreatedAt
        };
    }

    public async Task<List<ProductDto>> GetListForDropdownAsync(Guid? firmId, CancellationToken ct = default)
    {
        var list = await ScopeQuery(firmId)
            .OrderBy(x => x.Code)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Code = x.Code,
                Name = x.Name,
                Barcode = x.Barcode,
                Unit = x.Unit,
                StockQuantity = x.StockQuantity,
                UnitPrice = x.UnitPrice,
                Currency = x.Currency,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
        return list;
    }

    public async Task<List<ProductSaleRowDto>> GetProductSalesAsync(Guid productId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == productId, ct);
        if (!hasAccess) return new List<ProductSaleRowDto>();

        var query = _db.InvoiceItems
            .Where(i => i.ProductId == productId)
            .Join(_db.Invoices.Where(inv => inv.InvoiceType == InvoiceType.Satis), i => i.InvoiceId, inv => inv.Id, (i, inv) => new { i.Quantity, inv.InvoiceDate, inv.InvoiceNumber, inv.Id, inv.SourceType, inv.SourceId });
        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.Date;
            query = query.Where(x => x.InvoiceDate >= from);
        }
        if (dateTo.HasValue)
        {
            var toExclusive = dateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.InvoiceDate < toExclusive);
        }
        var rows = await query.OrderByDescending(x => x.InvoiceDate).ThenByDescending(x => x.Id).ToListAsync(ct);

        var dnIds = rows.Where(x => x.SourceType == ReferenceType.Irsaliye && x.SourceId.HasValue).Select(x => x.SourceId!.Value).Distinct().ToList();
        var dns = await _db.DeliveryNotes.Where(d => dnIds.Contains(d.Id)).Select(d => new { d.Id, d.DeliveryNumber, d.OrderId }).ToListAsync(ct);
        var dnDict = dns.ToDictionary(d => d.Id);

        var orderIds = dns.Where(d => d.OrderId.HasValue).Select(d => d.OrderId!.Value).Union(rows.Where(x => x.SourceType == ReferenceType.Siparis && x.SourceId.HasValue).Select(x => x.SourceId!.Value)).Distinct().ToList();
        var orders = await _db.Orders.Where(o => orderIds.Contains(o.Id)).Select(o => new { o.Id, o.OrderNumber }).ToListAsync(ct);
        var orderDict = orders.ToDictionary(o => o.Id);

        return rows.Select(r =>
        {
            string? orderNumber = null;
            Guid? orderId = null;
            string? deliveryNumber = null;
            Guid? deliveryNoteId = null;
            if (r.SourceType == ReferenceType.Irsaliye && r.SourceId.HasValue && dnDict.TryGetValue(r.SourceId.Value, out var dn))
            {
                deliveryNumber = dn.DeliveryNumber;
                deliveryNoteId = dn.Id;
                if (dn.OrderId.HasValue && orderDict.TryGetValue(dn.OrderId.Value, out var ord))
                {
                    orderNumber = ord.OrderNumber;
                    orderId = ord.Id;
                }
            }
            else if (r.SourceType == ReferenceType.Siparis && r.SourceId.HasValue && orderDict.TryGetValue(r.SourceId.Value, out var ord))
            {
                orderNumber = ord.OrderNumber;
                orderId = ord.Id;
            }
            return new ProductSaleRowDto
            {
                Date = r.InvoiceDate,
                Quantity = r.Quantity,
                InvoiceNumber = r.InvoiceNumber,
                InvoiceId = r.Id,
                OrderNumber = orderNumber,
                OrderId = orderId,
                DeliveryNumber = deliveryNumber,
                DeliveryNoteId = deliveryNoteId
            };
        }).ToList();
    }
}
