namespace LMS.API.Models;

public class ApiResponse
{
    public bool Success { get; }
    public int StatusCode { get; set; }
    public string? Message { get; }
    public object? Errors { get; }
    public object? Data { get; }

    private ApiResponse(bool success, int statusCode, string? message, object? errors, object? data)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
        Data = data;
    }

    public static ApiResponse Ok(string? message, object? data) 
        => new(true, StatusCodes.Status200OK, message,null , data);

    public static ApiResponse BadRequest(object? errors, string? message = null)
       => new(false, StatusCodes.Status400BadRequest, message, errors, null);

    public static ApiResponse NotFound(string? message)
        => new(false, StatusCodes.Status404NotFound, message, null, null);

    public static ApiResponse Unauthorized(string? message = "Unauthorized")
        => new(false, StatusCodes.Status401Unauthorized, message, null, null);

    public static ApiResponse Forbidden(string? message = "Forbidden")
        => new(false, StatusCodes.Status403Forbidden, message, null, null);

    public static ApiResponse Conflict(string message)
        => new(false, StatusCodes.Status409Conflict, message, null, null);

    public static ApiResponse InternalError(string? message)
        => new(false, StatusCodes.Status500InternalServerError, message, null, null);
}
