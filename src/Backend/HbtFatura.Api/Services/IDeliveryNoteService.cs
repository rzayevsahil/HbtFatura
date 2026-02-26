using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.DeliveryNotes;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public interface IDeliveryNoteService
{
    Task<PagedResult<DeliveryNoteListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, DeliveryNoteStatus? status, Guid? customerId, Guid? orderId, Guid? firmId, CancellationToken ct = default);
    Task<DeliveryNoteDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DeliveryNoteDto> CreateAsync(CreateDeliveryNoteRequest request, CancellationToken ct = default);
    Task<DeliveryNoteDto?> CreateFromOrderAsync(Guid orderId, DateTime deliveryDate, CancellationToken ct = default);
    Task<DeliveryNoteDto?> UpdateAsync(Guid id, UpdateDeliveryNoteRequest request, CancellationToken ct = default);
    Task<bool> SetStatusAsync(Guid id, DeliveryNoteStatus status, CancellationToken ct = default);
}
