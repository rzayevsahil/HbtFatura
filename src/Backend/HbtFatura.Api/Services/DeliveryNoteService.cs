using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.DeliveryNotes;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Helpers;

namespace HbtFatura.Api.Services;

public class DeliveryNoteService : IDeliveryNoteService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public DeliveryNoteService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<DeliveryNote> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.DeliveryNotes.Where(d => d.User != null && d.User.FirmId == firmIdFilter.Value);
            return _db.DeliveryNotes.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin)
            return _db.DeliveryNotes.Where(d => d.User != null && d.User.FirmId == _currentUser.FirmId);
        return _db.DeliveryNotes.Where(d => d.UserId == _currentUser.UserId);
    }

    public async Task<PagedResult<DeliveryNoteListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, DeliveryNoteStatus? status, Guid? customerId, Guid? orderId, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.DeliveryDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.DeliveryDate <= dateTo.Value.Date);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (orderId.HasValue) query = query.Where(x => x.OrderId == orderId.Value);

        var total = await query.CountAsync(ct);
        var list = await query
            .Include(x => x.Customer)
            .Include(x => x.Order)
            .OrderByDescending(x => x.DeliveryDate)
            .ThenByDescending(x => x.DeliveryNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DeliveryNoteListDto
            {
                Id = x.Id,
                DeliveryNumber = x.DeliveryNumber,
                DeliveryDate = x.DeliveryDate,
                Status = x.Status,
                DeliveryType = x.DeliveryType,
                CustomerTitle = x.Customer != null ? x.Customer.Title : null,
                OrderNumber = x.Order != null ? x.Order.OrderNumber : null,
                InvoiceId = x.InvoiceId
            })
            .ToListAsync(ct);
        return new PagedResult<DeliveryNoteListDto> { Items = list, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<DeliveryNoteDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var dn = await ScopeQuery()
            .Include(x => x.Customer)
            .Include(x => x.Order)
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return dn == null ? null : MapToDto(dn);
    }

    public async Task<DeliveryNoteDto> CreateAsync(CreateDeliveryNoteRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var deliveryDate = DateTimeHelper.NormalizeForStorage(request.DeliveryDate);
        var deliveryNumber = await GetNextDeliveryNumberAsync(userId, deliveryDate.Year, ct);

        string? customerTitle = null;
        Guid? orderId = request.OrderId;
        string? orderNumber = null;
        if (request.CustomerId.HasValue)
        {
            var customer = await _db.Customers
                .Where(c => c.Id == request.CustomerId && (c.UserId == userId || (_currentUser.IsFirmAdmin && c.User != null && c.User.FirmId == _currentUser.FirmId)))
                .Select(c => new { c.Title })
                .FirstOrDefaultAsync(ct);
            customerTitle = customer?.Title;
        }
        if (request.OrderId.HasValue)
        {
            var order = await _db.Orders.Where(o => o.Id == request.OrderId).Select(o => new { o.OrderNumber }).FirstOrDefaultAsync(ct);
            orderNumber = order?.OrderNumber;
        }

        var dn = new DeliveryNote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeliveryNumber = deliveryNumber,
            CustomerId = request.CustomerId,
            OrderId = orderId,
            DeliveryDate = deliveryDate.Date,
            Status = DeliveryNoteStatus.Taslak,
            DeliveryType = request.DeliveryType,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var sortOrder = 0;
        foreach (var item in request.Items ?? new List<DeliveryNoteItemInputDto>())
        {
            dn.Items.Add(new DeliveryNoteItem
            {
                Id = Guid.NewGuid(),
                DeliveryNoteId = dn.Id,
                ProductId = item.ProductId,
                OrderItemId = item.OrderItemId,
                Description = (item.Description ?? string.Empty).Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                SortOrder = sortOrder++
            });
        }

        _db.DeliveryNotes.Add(dn);
        await _db.SaveChangesAsync(ct);
        dn = (await ScopeQuery().Include(x => x.Customer).Include(x => x.Order).Include(x => x.Items.OrderBy(i => i.SortOrder)).FirstOrDefaultAsync(x => x.Id == dn.Id, ct))!;
        return MapToDto(dn);
    }

    public async Task<DeliveryNoteDto?> CreateFromOrderAsync(Guid orderId, DateTime deliveryDateParam, CancellationToken ct = default)
    {
        var deliveryDate = DateTimeHelper.NormalizeForStorage(deliveryDateParam);
        var order = await _db.Orders
            .Include(x => x.User)
            .Include(x => x.Customer)
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(x => x.Id == orderId, ct);
        if (order == null) return null;
        if (!_currentUser.IsSuperAdmin && !(_currentUser.IsFirmAdmin && order.User?.FirmId == _currentUser.FirmId) && order.UserId != _currentUser.UserId)
            return null;
        if (order.Status == OrderStatus.TamamiTeslim || order.Status == OrderStatus.KismiTeslim)
            throw new InvalidOperationException("Bu sipariş zaten irsaliyeye dönüştürülmüş veya kısmen teslim edilmiş.");
        if (order.Status == OrderStatus.Iptal)
            throw new InvalidOperationException("İptal edilmiş siparişten irsaliye oluşturulamaz.");

        var userId = _currentUser.UserId;
        var deliveryNumber = await GetNextDeliveryNumberAsync(userId, deliveryDate.Year, ct);

        var dn = new DeliveryNote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeliveryNumber = deliveryNumber,
            CustomerId = order.CustomerId,
            OrderId = order.Id,
            DeliveryDate = deliveryDate.Date,
            Status = DeliveryNoteStatus.Taslak,
            DeliveryType = order.OrderType,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        var sortOrder = 0;
        foreach (var orderItem in order.Items)
        {
            dn.Items.Add(new DeliveryNoteItem
            {
                Id = Guid.NewGuid(),
                DeliveryNoteId = dn.Id,
                ProductId = orderItem.ProductId,
                OrderItemId = orderItem.Id,
                Description = orderItem.Description,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                VatRate = orderItem.VatRate,
                SortOrder = sortOrder++
            });
        }

        _db.DeliveryNotes.Add(dn);
        order.Status = OrderStatus.TamamiTeslim;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = userId;
        await _db.SaveChangesAsync(ct);

        dn = (await ScopeQuery().Include(x => x.Customer).Include(x => x.Order).Include(x => x.Items.OrderBy(i => i.SortOrder)).FirstOrDefaultAsync(x => x.Id == dn.Id, ct))!;
        return MapToDto(dn);
    }

    public async Task<DeliveryNoteDto?> UpdateAsync(Guid id, UpdateDeliveryNoteRequest request, CancellationToken ct = default)
    {
        var dn = await ScopeQuery().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (dn == null) return null;
        if (dn.InvoiceId.HasValue)
            throw new InvalidOperationException("Faturaya aktarılmış irsaliye güncellenemez.");
        if (dn.Status == DeliveryNoteStatus.Onaylandi)
            throw new InvalidOperationException("Onaylanmış irsaliye güncellenemez.");
        if (dn.Status == DeliveryNoteStatus.Iptal)
            throw new InvalidOperationException("İptal edilmiş irsaliye güncellenemez.");

        dn.CustomerId = request.CustomerId;
        dn.DeliveryDate = DateTimeHelper.NormalizeForStorage(request.DeliveryDate).Date;
        dn.UpdatedAt = DateTime.UtcNow;
        dn.UpdatedBy = _currentUser.UserId;

        _db.DeliveryNoteItems.RemoveRange(dn.Items);
        var sortOrder = 0;
        foreach (var item in request.Items ?? new List<DeliveryNoteItemInputDto>())
        {
            dn.Items.Add(new DeliveryNoteItem
            {
                Id = Guid.NewGuid(),
                DeliveryNoteId = dn.Id,
                ProductId = item.ProductId,
                OrderItemId = item.OrderItemId,
                Description = (item.Description ?? string.Empty).Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate,
                SortOrder = sortOrder++
            });
        }

        await _db.SaveChangesAsync(ct);
        dn = (await ScopeQuery().Include(x => x.Customer).Include(x => x.Order).Include(x => x.Items.OrderBy(i => i.SortOrder)).FirstOrDefaultAsync(x => x.Id == id, ct))!;
        return MapToDto(dn);
    }

    public async Task<bool> SetStatusAsync(Guid id, DeliveryNoteStatus status, CancellationToken ct = default)
    {
        var dn = await ScopeQuery().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (dn == null) return false;
        if (dn.Status == DeliveryNoteStatus.Onaylandi && status != DeliveryNoteStatus.Onaylandi)
            return false;

        if (status == DeliveryNoteStatus.Onaylandi)
        {
            var alreadyCreated = await _db.StockMovements.AnyAsync(m => m.ReferenceType == ReferenceType.Irsaliye && m.ReferenceId == id, ct);
            if (!alreadyCreated)
            {
                var direction = dn.DeliveryType == InvoiceType.Alis ? StockMovementType.Giris : StockMovementType.Cikis;
                foreach (var item in dn.Items.Where(i => i.ProductId.HasValue))
                {
                    _db.StockMovements.Add(new StockMovement
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId!.Value,
                        Date = dn.DeliveryDate,
                        Type = direction,
                        Quantity = item.Quantity,
                        ReferenceType = ReferenceType.Irsaliye,
                        ReferenceId = id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        dn.Status = status;
        dn.UpdatedAt = DateTime.UtcNow;
        dn.UpdatedBy = _currentUser.UserId;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<string> GetNextDeliveryNumberAsync(Guid userId, int year, CancellationToken ct)
    {
        var last = await _db.DeliveryNotes
            .Where(x => x.UserId == userId && x.DeliveryDate.Year == year)
            .OrderByDescending(x => x.DeliveryNumber)
            .Select(x => x.DeliveryNumber)
            .FirstOrDefaultAsync(ct);
        if (string.IsNullOrEmpty(last))
            return $"IRS-{year}-0001";
        var parts = last.Split('-');
        if (parts.Length != 3 || !int.TryParse(parts[2], out var num))
            return $"IRS-{year}-0001";
        return $"IRS-{year}-{(num + 1):D4}";
    }

    private static DeliveryNoteDto MapToDto(DeliveryNote d) => new()
    {
        Id = d.Id,
        DeliveryNumber = d.DeliveryNumber,
        CustomerId = d.CustomerId,
        CustomerTitle = d.Customer?.Title,
        OrderId = d.OrderId,
        OrderNumber = d.Order?.OrderNumber,
        InvoiceId = d.InvoiceId,
        DeliveryDate = d.DeliveryDate,
        Status = d.Status,
        DeliveryType = d.DeliveryType,
        CreatedAt = d.CreatedAt,
        Items = d.Items.OrderBy(x => x.SortOrder).Select(x => new DeliveryNoteItemDto
        {
            Id = x.Id,
            ProductId = x.ProductId,
            OrderItemId = x.OrderItemId,
            Description = x.Description,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            VatRate = x.VatRate,
            SortOrder = x.SortOrder
        }).ToList()
    };
}
