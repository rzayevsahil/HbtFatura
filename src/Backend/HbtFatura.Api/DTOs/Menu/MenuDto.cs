namespace HbtFatura.Api.DTOs.Menu;

public class MenuDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? RouterLink { get; set; }
    public int SortOrder { get; set; }
    public string? RequiredPermissionCode { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemMenu { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}
