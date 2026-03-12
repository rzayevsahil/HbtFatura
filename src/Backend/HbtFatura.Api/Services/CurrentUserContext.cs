using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;

namespace HbtFatura.Api.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "";
    public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public Guid? FirmId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst("FirmId")?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? FirmName => _httpContextAccessor.HttpContext?.User?.FindFirst("FirmName")?.Value;
    public bool IsAuthenticated => UserId != Guid.Empty;

    public bool IsSuperAdmin => Role == Roles.SuperAdmin;
    public bool IsFirmAdmin => Role == Roles.FirmAdmin;
    public bool IsEmployee => Role == Roles.Employee;

    public async Task<bool> HasPermissionAsync(string permissionCode, AppDbContext db, CancellationToken ct = default)
    {
        if (IsSuperAdmin) return true;
        
        var userId = UserId;
        if (userId == Guid.Empty) return false;

        return await (from ur in db.UserRoles
                      join rp in db.RolePermissions on ur.RoleId equals rp.RoleId
                      join p in db.Permissions on rp.PermissionId equals p.Id
                      where ur.UserId == userId && p.Code == permissionCode
                      select p.Id).AnyAsync(ct);
    }
}
