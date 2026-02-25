using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public interface ICustomerService
{
    Task<PagedResult<CustomerListDto>> GetPagedAsync(int page, int pageSize, string? search, Guid? firmId, CancellationToken ct = default);
    Task<List<CustomerDto>> GetListForDropdownAsync(CancellationToken ct = default);
    Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<decimal> GetBalanceAsync(Guid customerId, CancellationToken ct = default);
    Task<PagedResult<AccountTransactionDto>> GetTransactionsAsync(Guid customerId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct = default);
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
