using HbtFatura.Api.Data;

namespace HbtFatura.Api.Services;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string Role { get; }
    string? FullName { get; }
    Guid? FirmId { get; }
    string? FirmName { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    bool IsFirmAdmin { get; }
    bool IsEmployee { get; }
    Task<bool> HasPermissionAsync(string permissionCode, AppDbContext db, CancellationToken ct = default);
}
