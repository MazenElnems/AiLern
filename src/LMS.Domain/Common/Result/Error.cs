namespace LMS.Domain.Common.Result;
public record Error(string Title, ErrorType ErrorType, string Description);

public enum ErrorType
{
    Validation,
    NotFound,
    Unauthorized,
    Forbidden
}