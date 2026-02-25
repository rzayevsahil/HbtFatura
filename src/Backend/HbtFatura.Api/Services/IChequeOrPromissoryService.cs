using HbtFatura.Api.DTOs.Cheque;
using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public interface IChequeOrPromissoryService
{
    Task<PagedResult<ChequeOrPromissoryDto>> GetPagedAsync(int page, int pageSize, int? type, int? status, Guid? customerId, Guid? firmId, DateTime? dueFrom, DateTime? dueTo, CancellationToken ct = default);
    Task<ChequeOrPromissoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ChequeOrPromissoryDto> CreateAsync(CreateChequeOrPromissoryRequest request, CancellationToken ct = default);
    Task<ChequeOrPromissoryDto?> UpdateAsync(Guid id, UpdateChequeOrPromissoryRequest request, CancellationToken ct = default);
    Task<bool> SetStatusAsync(Guid id, int status, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
