namespace LMS.Domain.Enums;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    BusinessRule,
    Conflict
}