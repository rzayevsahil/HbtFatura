using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Orders;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Helpers;

namespace HbtFatura.Api.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public OrderService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<Order> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.Orders.Where(o => o.User != null && o.User.FirmId == firmIdFilter.Value);
            return _db.Orders.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin)
            return _db.Orders.Where(o => o.User != null && o.User.FirmId == _currentUser.FirmId);
        return _db.Orders.Where(o => o.UserId == _currentUser.UserId);
    }

    public async Task<PagedResult<OrderListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status, Guid? customerId, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.OrderDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.OrderDate < dateTo.Value.Date.AddDays(1));
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);

        var total = await query.CountAsync(ct);
        var orders = await query
            .Include(x => x.Customer)
            .Include(x => x.Items)
            .OrderByDescending(x => x.OrderDate)
            .ThenByDescending(x => x.OrderNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = orders.Select(o => new OrderListDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            Status = o.Status,
            OrderType = o.OrderType,
            CustomerTitle = o.Customer != null ? o.Customer.Title : null,
            TotalAmount = o.Items.Sum(i => i.Quantity * i.UnitPrice * (1 + i.VatRate / 100m))
        }).ToList();

        return new PagedResult<OrderListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await ScopeQuery()
            .Include(x => x.Customer)
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var orderDate = DateTimeHelper.NormalizeForStorage(request.OrderDate);
        var orderNumber = await GetNextOrderNumberAsync(userId, orderDate.Year, ct);

        string? customerTitle = null;
        if (request.CustomerId.HasValue)
        {
            var customer = await _db.Customers
                .Where(c => c.Id == request.CustomerId && (c.UserId == userId || (_currentUser.IsFirmAdmin && c.User != null && c.User.FirmId == _currentUser.FirmId)))
                .Select(c => new { c.Title })
                .FirstOrDefaultAsync(ct);
            customerTitle = customer?.Title;
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderNumber = orderNumber,
            CustomerId = request.CustomerId,
            OrderDate = orderDate,
            Status = OrderStatus.Bekliyor,
            OrderType = request.OrderType,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var sortOrder = 0;
        foreach (var item in request.Items)
        {
            if (request.OrderType == InvoiceType.Satis && item.ProductId.HasValue)
            {
                var product = await _db.Products.FindAsync(item.ProductId.Value);
                if (product != null && item.Quantity > product.StockQuantity)
                    throw new InvalidOperationException($"'{product.Name}' ürünü için yetersiz stok! Mevcut: {product.StockQuantity}");
            }

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Description = item.Description.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                SortOrder = sortOrder++
            });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        order = (await ScopeQuery().Include(x => x.Customer).Include(x => x.Items.OrderBy(i => i.SortOrder)).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == order.Id, ct))!;
        return MapToDto(order);
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken ct = default)
    {
        var order = await ScopeQuery().AsTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order == null) return null;
        if (order.Status != OrderStatus.Bekliyor)
            throw new InvalidOperationException("Sadece bekleyen (taslak) sipariş düzenlenebilir.");

        // Mevcut kalemleri ayrı sorguyla yükle ve EF üzerinden sil (loglama/audit için doğru yol)
        var existingItems = await _db.OrderItems.Where(x => x.OrderId == id).ToListAsync(ct);
        _db.OrderItems.RemoveRange(existingItems);

        order.CustomerId = request.CustomerId;
        order.OrderDate = DateTimeHelper.NormalizeForStorage(request.OrderDate);
        if (request.Status.HasValue && (request.Status.Value == OrderStatus.Bekliyor || request.Status.Value == OrderStatus.Onaylandi))
            order.Status = request.Status.Value;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = _currentUser.UserId;

        var sortOrder = 0;
        foreach (var item in request.Items ?? new List<OrderItemInputDto>())
        {
            if (order.OrderType == InvoiceType.Satis && item.ProductId.HasValue)
            {
                var product = await _db.Products.FindAsync(item.ProductId.Value);
                if (product != null && item.Quantity > product.StockQuantity)
                    throw new InvalidOperationException($"'{product.Name}' ürünü için yetersiz stok! Mevcut: {product.StockQuantity}");
            }

            _db.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Description = (item.Description ?? string.Empty).Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                SortOrder = sortOrder++
            });
        }

        await _db.SaveChangesAsync(ct);
        order = (await ScopeQuery().Include(x => x.Customer).Include(x => x.Items.OrderBy(i => i.SortOrder)).ThenInclude(i => i.Product).FirstOrDefaultAsync(x => x.Id == id, ct))!;
        return MapToDto(order);
    }

    public async Task<bool> SetStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        var order = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order == null) return false;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = _currentUser.UserId;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<string> GetNextOrderNumberAsync(Guid userId, int year, CancellationToken ct)
    {
        var last = await _db.Orders
            .Where(x => x.UserId == userId && x.OrderDate.Year == year)
            .OrderByDescending(x => x.OrderNumber)
            .Select(x => x.OrderNumber)
            .FirstOrDefaultAsync(ct);
        if (string.IsNullOrEmpty(last))
            return $"SIP-{year}-0001";
        var parts = last.Split('-');
        if (parts.Length != 3 || !int.TryParse(parts[2], out var num))
            return $"SIP-{year}-0001";
        return $"SIP-{year}-{(num + 1):D4}";
    }

    private static OrderDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        CustomerId = o.CustomerId,
        CustomerTitle = o.Customer?.Title,
        OrderDate = o.OrderDate,
        Status = o.Status,
        OrderType = o.OrderType,
        CreatedAt = o.CreatedAt,
        Items = o.Items.OrderBy(x => x.SortOrder).Select(x => new OrderItemDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            ProductCode = x.Product?.Code,
            Description = x.Description,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            VatRate = x.VatRate,
            SortOrder = x.SortOrder
        }).ToList()
    };
}
