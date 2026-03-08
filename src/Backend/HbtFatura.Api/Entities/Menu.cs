namespace HbtFatura.Api.Entities;

public class Menu
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? RouterLink { get; set; }
    public int SortOrder { get; set; }
    public string? RequiredPermissionCode { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystemMenu { get; set; }

    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
}
