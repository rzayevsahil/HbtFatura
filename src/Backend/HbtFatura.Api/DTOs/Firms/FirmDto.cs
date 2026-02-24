namespace HbtFatura.Api.DTOs.Firms;

public class FirmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateFirmRequest
{
    public string Name { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminFullName { get; set; } = string.Empty;
}

public class UpdateFirmRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>Firmaya bağlı kullanıcı (FirmAdmin + çalışanlar). SuperAdmin firma detayında listelenir.</summary>
public class FirmUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
