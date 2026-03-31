using Microsoft.AspNetCore.Identity;

namespace HbtFatura.Api.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
    
    public string? DisplayName { get; set; }

    /// <summary>Seed / sistem rolü; silinemez (SuperAdmin, Firma Yöneticisi, Çalışan).</summary>
    public bool IsSystem { get; set; }
}
