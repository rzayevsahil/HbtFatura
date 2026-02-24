namespace HbtFatura.Api.DTOs.Auth;

/// <summary>
/// Backend-only register: SuperAdmin (sadece hiç kullanıcı yokken), FirmAdmin (SuperAdmin + firmId), Employee (FirmAdmin).
/// Frontend'de register sayfası yok; bu endpoint Swagger/Postman veya diğer servisler için.
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    /// <summary>SuperAdmin | FirmAdmin | Employee</summary>
    public string Role { get; set; } = string.Empty;
    /// <summary>FirmAdmin için zorunlu; kullanıcının atanacağı firma Id.</summary>
    public Guid? FirmId { get; set; }
}
