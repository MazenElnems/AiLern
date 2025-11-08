using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LMS.Core.CurrentUser;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserIdentity? GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if(httpContext.User.Identity is null || !httpContext.User.Identity.IsAuthenticated)
        {
            return null;
        }

        var id = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = httpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value);

        return new CurrentUserIdentity(Convert.ToInt32(id), userName!, email!, roles);
    }
}
