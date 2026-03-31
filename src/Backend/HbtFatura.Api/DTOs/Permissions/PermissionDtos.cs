namespace HbtFatura.Api.DTOs.Permissions;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsSystem { get; set; }
}

public class RolePermissionsDto
{
    public Guid RoleId { get; set; }
    public List<string> PermissionCodes { get; set; } = new();
}

public class UpdateRolePermissionsRequest
{
    public List<string> PermissionCodes { get; set; } = new();
}
