using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HbtFatura.Api.Constants;

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

    public Guid? FirmId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst("FirmId")?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? FirmName => _httpContextAccessor.HttpContext?.User?.FindFirst("FirmName")?.Value;

    public bool IsSuperAdmin => Role == Roles.SuperAdmin;
    public bool IsFirmAdmin => Role == Roles.FirmAdmin;
    public bool IsEmployee => Role == Roles.Employee;
}
