using HbtFatura.Api.DTOs.AccountPayment;
using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public interface IAccountPaymentService
{
    Task<PagedResult<AccountPaymentListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, Guid? customerId, string? type, Guid? firmId, CancellationToken ct = default);
    Task CreateAsync(AccountPaymentRequest request, CancellationToken ct = default);
}
