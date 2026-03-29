using HbtFatura.Api.DTOs.Validation;

namespace HbtFatura.Api.Services;

public interface ITaxNumberUniquenessService
{
    Task<TaxNumberCheckResponseDto> CheckAsync(
        string? value,
        TaxNumberCheckMode mode,
        Guid? excludeCustomerId,
        Guid? excludeFirmIdForCompanyRow,
        CancellationToken ct = default);

    Task EnsureUniqueForCustomerAsync(string? taxNumber, Guid? excludeCustomerId, CancellationToken ct = default);

    Task EnsureUniqueForCompanyAsync(string? taxNumber, Guid firmId, CancellationToken ct = default);
}
