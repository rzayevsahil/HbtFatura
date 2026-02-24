using HbtFatura.Api.DTOs.Firms;

namespace HbtFatura.Api.Services;

public interface IFirmService
{
    Task<IReadOnlyList<FirmDto>> GetAllAsync(CancellationToken ct = default);
    Task<FirmDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FirmDto> CreateAsync(CreateFirmRequest request, CancellationToken ct = default);
    Task<FirmDto?> UpdateAsync(Guid id, UpdateFirmRequest request, CancellationToken ct = default);
}
