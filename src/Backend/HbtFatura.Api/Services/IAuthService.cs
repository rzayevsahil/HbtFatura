using HbtFatura.Api.DTOs.Auth;

namespace HbtFatura.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default);
    /// <summary>Backend-only: SuperAdmin (sadece hiç kullanıcı yokken), FirmAdmin (SuperAdmin + firmId), Employee (FirmAdmin).</summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, string? ipAddress, CancellationToken ct = default);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default);
}
