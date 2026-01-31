using LMS.API.Common.Responses;
using LMS.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace LMS.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch(ValidationException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning(ex, "Validation failed.");
        }
        catch (ArgumentException ex)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning(ex, "Invalid request parameters.");
        }
        catch(UnauthorizedAccessException ex)
        {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync(ex.Message);
            _logger.LogWarning("Access forbidden.");
        }
        catch(PasswordResetException)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("can't reset the password, please try again!");
            _logger.LogWarning("Password reset failed.");
        }
        catch(EmailConfirmationException)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Email confirmation failed.");
            _logger.LogWarning("Email confirmation failed.");
        }
        catch (CourseUpdateException ex)
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
        catch(CourseEnrollmentException)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Cannot enroll in the course.");
            _logger.LogWarning("Course enrollment failed.");
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError(ex, "An unhandled exception occurred.");
            await httpContext.Response.WriteAsJsonAsync(ApiResponse.InternalError(ex.Message));
        }
    }
}

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
