using HbtFatura.Api.DTOs.Bank;
using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public interface IBankAccountService
{
    Task<IReadOnlyList<BankAccountDto>> GetAllAsync(Guid? firmId, CancellationToken ct = default);
    Task<BankAccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BankAccountDto> CreateAsync(CreateBankAccountRequest request, CancellationToken ct = default);
    Task<BankAccountDto?> UpdateAsync(Guid id, UpdateBankAccountRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<BankTransactionDto>> GetTransactionsAsync(Guid bankAccountId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<BankTransactionDto> AddTransactionAsync(Guid bankAccountId, CreateBankTransactionRequest request, CancellationToken ct = default);
}
