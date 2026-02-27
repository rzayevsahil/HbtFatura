using HbtFatura.Api.DTOs.Users;

namespace HbtFatura.Api.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
}
