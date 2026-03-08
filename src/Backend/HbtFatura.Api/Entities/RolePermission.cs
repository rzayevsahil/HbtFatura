using Microsoft.AspNetCore.Identity;

namespace HbtFatura.Api.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public IdentityRole<Guid> Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
