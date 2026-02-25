using HbtFatura.Api.DTOs.AccountPayment;

namespace HbtFatura.Api.Services;

public interface IAccountPaymentService
{
    Task CreateAsync(AccountPaymentRequest request, CancellationToken ct = default);
}
