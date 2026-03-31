namespace HbtFatura.Api.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Group { get; set; } = string.Empty; // e.g. "Faturalar"
    public string Code { get; set; } = string.Empty;  // e.g. "Invoices.View"
    public string Name { get; set; } = string.Empty;  // e.g. "Faturaları Görüntüle"
    public string? Description { get; set; }

    /// <summary>Seed / sistem yetkisi; silinemez.</summary>
    public bool IsSystem { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
