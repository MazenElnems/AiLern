using LMS.Domain.Enums;

namespace LMS.Domain.Errors;

public sealed record Error(ErrorType Type, string Title, string Message)
{
    public static readonly Error None = new(ErrorType.None, string.Empty, string.Empty);

    public static Error Validation(string title, string message) =>
        new(ErrorType.Validation, title, message);

    public static Error NotFound(string title, string message) =>
        new(ErrorType.NotFound, title, message);

    public static Error Unauthorized(string title, string message) =>
        new(ErrorType.Unauthorized, title, message);

    public static Error Forbidden(string title, string message) =>
        new(ErrorType.Forbidden, title, message);

    public static Error BusinessRule(string title, string message) =>
        new(ErrorType.BusinessRule, title, message);

    public static Error Conflict(string title, string message) =>
        new(ErrorType.Conflict, title, message);
}
