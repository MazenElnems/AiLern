using LMS.API.Models;

namespace LMS.API.Middleware;

public class AuthResponseMiddleware
{
    private readonly RequestDelegate _next;

    public AuthResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {

        await _next(httpContext);

        switch(httpContext.Response.StatusCode)
        {
            case StatusCodes.Status401Unauthorized:
                await httpContext.Response.WriteAsJsonAsync(
                    ApiResponse.Unauthorized("user not unauthenticated. please provide valid credentials.")
                );
                break;
            case StatusCodes.Status403Forbidden:
                await httpContext.Response.WriteAsJsonAsync(
                    ApiResponse.Forbidden("user not authorized. you are not allowed to access this resource.")
                );
                break;
            default:
                break;
        }
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAuthResponseHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthResponseMiddleware>();
    }
}
