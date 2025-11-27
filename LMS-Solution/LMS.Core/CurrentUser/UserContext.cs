using LMS.Domin.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LMS.Core.CurrentUser;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserIdentity GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User.Identity is null || !httpContext.User.Identity.IsAuthenticated)
            throw new UnAuthorizedException("User is not authenticated");

        var id = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = httpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value);

        return new CurrentUserIdentity(Convert.ToInt32(id), userName!, email!, roles);
    }
}