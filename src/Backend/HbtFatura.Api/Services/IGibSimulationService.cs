using HbtFatura.Api.DTOs.GibSimulation;

namespace HbtFatura.Api.Services;

public interface IGibSimulationService
{
    Task<IReadOnlyList<GibInboxItemDto>> GetInboxAsync(CancellationToken ct = default);
    Task AcceptAsync(Guid submissionId, CancellationToken ct = default);
    Task RejectAsync(Guid submissionId, CancellationToken ct = default);
}
