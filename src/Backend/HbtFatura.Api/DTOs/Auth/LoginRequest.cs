namespace HbtFatura.Api.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    /// <summary>İşaretliyse refresh token daha uzun ömürlü (Jwt:RefreshTokenExpirationDaysRememberMe).</summary>
    public bool RememberMe { get; set; }
}
