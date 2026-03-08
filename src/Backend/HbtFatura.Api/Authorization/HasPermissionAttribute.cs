using Microsoft.AspNetCore.Authorization;

namespace HbtFatura.Api.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}
