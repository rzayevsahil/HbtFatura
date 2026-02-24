using HbtFatura.Api.DTOs.CompanySettings;

namespace HbtFatura.Api.Services;

public interface ICompanySettingsService
{
    Task<CompanySettingsDto?> GetByFirmIdAsync(Guid? firmId, CancellationToken ct = default);
    Task<CompanySettingsDto?> CreateOrUpdateAsync(UpdateCompanySettingsRequest request, Guid? firmId, CancellationToken ct = default);
}
