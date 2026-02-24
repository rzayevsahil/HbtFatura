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
