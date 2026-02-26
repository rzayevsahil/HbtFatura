using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Product;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public ProductService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<Product> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.Products.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.Products.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.Products.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.Products.Where(x => false);
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
                CreatedAt = x.CreatedAt,
                StockQuantity = x.StockQuantity
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
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        Guid firmId;
        if (_currentUser.IsSuperAdmin && request.FirmId.HasValue)
            firmId = request.FirmId.Value;
        else if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        if (request.StockQuantity < 0) throw new ArgumentException("Başlangıç stok miktarı sıfırdan küçük olamaz.");

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            Barcode = request.Barcode?.Trim(),
            Unit = request.Unit?.Trim() ?? "Adet",
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };
        _db.Products.Add(entity);
        await _db.SaveChangesAsync(ct);
        
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
        entity.Code = request.Code.Trim();
        entity.Name = request.Name.Trim();
        entity.Barcode = request.Barcode?.Trim();
        entity.Unit = request.Unit?.Trim() ?? "Adet";
        
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

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<StockMovementDto>> GetMovementsAsync(Guid productId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == productId, ct);
        if (!hasAccess) return new PagedResult<StockMovementDto> { Items = new List<StockMovementDto>(), TotalCount = 0, Page = page, PageSize = pageSize };

        var query = _db.StockMovements.Where(m => m.ProductId == productId);
        if (dateFrom.HasValue) query = query.Where(m => m.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(m => m.Date <= dateTo.Value.Date);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new StockMovementDto
            {
                Id = m.Id,
                Date = m.Date,
                Type = m.Type,
                Quantity = m.Quantity,
                ReferenceType = m.ReferenceType,
                ReferenceId = m.ReferenceId,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(ct);
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
                CreatedAt = x.CreatedAt,
                StockQuantity = x.StockQuantity
            })
            .ToListAsync(ct);
        return list;
    }
}
