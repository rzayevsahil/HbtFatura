using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Invoices;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public interface IInvoiceService
{
    Task<PagedResult<InvoiceListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, InvoiceStatus? status, InvoiceType? invoiceType, Guid? customerId, Guid? firmId, CancellationToken ct = default);
    Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, CancellationToken ct = default);
    Task<InvoiceDto?> CreateFromDeliveryNoteAsync(Guid deliveryNoteId, CancellationToken ct = default);
    Task<InvoiceDto?> UpdateAsync(Guid id, UpdateInvoiceRequest request, byte[]? rowVersion, CancellationToken ct = default);
    Task<bool> SetStatusAsync(Guid id, InvoiceStatus status, CancellationToken ct = default);
}
