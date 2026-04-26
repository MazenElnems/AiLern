using LMS.Domain.Errors;

namespace LMS.Application.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    Error? Error { get;  }
    Dictionary<string, string[]>? ValidationErrors { get; }

    static abstract IResult Failure(Error error);
}
