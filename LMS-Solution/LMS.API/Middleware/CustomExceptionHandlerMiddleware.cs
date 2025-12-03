using LMS.Domin.Exceptions;

namespace LMS.API.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch(CourseUpdateException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning("Course update failed.");
        }
        catch (UnAuthorizedException ex)
        {
            httpContext.Response.StatusCode = 401;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning("User not authenticated");
        }
        catch (InvalidUserEmailOrPasswordException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning("Invalid User Login Attempts");
        }
        catch(ResourceNotFoundException ex)
        {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning(ex, "Resource not found.");
        }
        catch (UserCreationException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning(ex, "this user is already exists");
        }
        catch(CourseEnrollmentException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Cannot enroll in the course.");
            _logger.LogWarning("Course enrollment failed.");
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("something went wrong. Please try again later.");
            _logger.LogError(ex, "An unhandled exception occurred.");
        }
    }
}

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}
