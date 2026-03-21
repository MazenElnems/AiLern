using LMS.Domain.Errors;

namespace LMS.Application.Common.Results;

public interface IResult
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public Error? Error { get;  }
    public Dictionary<string, string[]>? ValidationErrors { get; }
}
