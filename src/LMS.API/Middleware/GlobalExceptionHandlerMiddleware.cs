using LMS.API.Models.Responses;
using LMS.Domain.Exceptions;

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
        catch(UnAuthorizedException ex)
        {
            _logger.LogError("Unauthorized Exception Was Thrown {@StackTrace} {@Exception}",
                ex.StackTrace,
                ex.Source
            );  

            await httpContext.Response.WriteAsJsonAsync(ApiResponse.Unauthorized("user not unauthenticated. please provide valid credentials."));
        }
        catch(AIServiceUnAvailableException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            _logger.LogError("AI Service Unavailable Exception Was Thrown {@StackTrace} {@Exception}",
                ex.StackTrace,
                ex.Source
            );

            await httpContext.Response.WriteAsJsonAsync(ApiResponse.InternalError("AI service is temporarily unavailable. Please try again later.", StatusCodes.Status503ServiceUnavailable));
        }
        catch(AIServiceException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError("AI Service Exception Was Thrown {@StackTrace} {@Exception}",
                ex.StackTrace,
                ex.Source
            );

            await httpContext.Response.WriteAsJsonAsync(ApiResponse.InternalError("AI Service Exception", StatusCodes.Status500InternalServerError));
        }
        catch (AIServiceTimeoutException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            _logger.LogError("AI Service Timeout Exception Was Thrown {@StackTrace} {@Exception}",
                ex.StackTrace,
                ex.Source
            );

            await httpContext.Response.WriteAsJsonAsync(ApiResponse.InternalError("AI Service request timed out. Please try again later.", StatusCodes.Status504GatewayTimeout));
        }
        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError("Exception Was Thrown {@StackTrace} {@ExceptionSource}",
                ex.StackTrace,
                ex.Source
            );

            await httpContext.Response.WriteAsJsonAsync(ApiResponse.InternalError("something went wrong.", StatusCodes.Status500InternalServerError));
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
