namespace HbtFatura.Api.Entities;

public class Lookup
{
    public Guid Id { get; set; }
    
    public Guid LookupGroupId { get; set; }
    public LookupGroup? Group { get; set; }

    public string Code { get; set; } = string.Empty;  // e.g. "0", "1", "Bekliyor"
    public string Name { get; set; } = string.Empty;  // e.g. "Alınan", "Verilen", "Bekliyor"
    public string? Color { get; set; }               // hex or CSS class
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
