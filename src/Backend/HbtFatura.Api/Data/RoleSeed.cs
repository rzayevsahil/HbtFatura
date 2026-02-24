using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Data;

public static class RoleSeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, CancellationToken ct = default)
    {
        foreach (var roleName in new[] { Roles.SuperAdmin, Roles.FirmAdmin, Roles.Employee })
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    public static async Task BootstrapSuperAdminIfEmptyAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        CancellationToken ct = default)
    {
        if (await userManager.Users.AnyAsync(ct))
            return;

        var email = config["Bootstrap:AdminEmail"];
        var password = config["Bootstrap:AdminPassword"];
        var fullName = config["Bootstrap:AdminFullName"] ?? "Super Admin";
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        email = email.Trim().ToLowerInvariant();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = fullName.Trim(),
            CreatedAt = DateTime.UtcNow,
            FirmId = null
        };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return;
        await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
    }
}
