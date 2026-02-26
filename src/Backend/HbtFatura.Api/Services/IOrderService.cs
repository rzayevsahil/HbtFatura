using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Orders;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public interface IOrderService
{
    Task<PagedResult<OrderListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status, Guid? customerId, Guid? firmId, CancellationToken ct = default);
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken ct = default);
    Task<bool> SetStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
}
