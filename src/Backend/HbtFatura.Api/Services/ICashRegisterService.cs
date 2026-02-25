using HbtFatura.Api.DTOs.Cash;
using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public interface ICashRegisterService
{
    Task<IReadOnlyList<CashRegisterDto>> GetAllAsync(Guid? firmId, CancellationToken ct = default);
    Task<CashRegisterDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CashRegisterDto> CreateAsync(CreateCashRegisterRequest request, CancellationToken ct = default);
    Task<CashRegisterDto?> UpdateAsync(Guid id, UpdateCashRegisterRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<CashTransactionDto>> GetTransactionsAsync(Guid cashRegisterId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<CashTransactionDto> AddTransactionAsync(Guid cashRegisterId, CreateCashTransactionRequest request, CancellationToken ct = default);
}
