using HbtFatura.Api.DTOs.MainAccountCode;

namespace HbtFatura.Api.Services;

public interface IMainAccountCodeService
{
    Task<IReadOnlyList<MainAccountCodeDto>> GetByFirmAsync(Guid? firmId, CancellationToken ct = default);
    Task<MainAccountCodeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MainAccountCodeDto> CreateAsync(CreateMainAccountCodeRequest request, CancellationToken ct = default);
    Task<MainAccountCodeDto?> UpdateAsync(Guid id, UpdateMainAccountCodeRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
