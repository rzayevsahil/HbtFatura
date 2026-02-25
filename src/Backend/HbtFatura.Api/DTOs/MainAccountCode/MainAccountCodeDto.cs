namespace HbtFatura.Api.DTOs.MainAccountCode;

public class MainAccountCodeDto
{
    public Guid Id { get; set; }
    public Guid? FirmId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>True when FirmId is null (sistem kodu - salt okunur).</summary>
    public bool IsSystem { get; set; }
}

public class CreateMainAccountCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public Guid? FirmId { get; set; }
}

public class UpdateMainAccountCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
