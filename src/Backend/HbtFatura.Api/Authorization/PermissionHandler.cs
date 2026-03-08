using System.Security.Claims;
using HbtFatura.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null) return;

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) return;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Check if any of the user's roles have the required permission
        var hasPermission = await (from ur in db.UserRoles
                                   join rp in db.RolePermissions on ur.RoleId equals rp.RoleId
                                   join p in db.Permissions on rp.PermissionId equals p.Id
                                   where ur.UserId == userId && p.Code == requirement.Permission
                                   select p).AnyAsync();

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
