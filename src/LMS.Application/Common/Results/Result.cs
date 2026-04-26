using LMS.Domain.Errors;

namespace LMS.Application.Common.Results;

public class Result : IResult
{
    public bool IsSuccess { get; init; }
    public string? Message { get; private set; }
    public Error? Error { get; private init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    protected Result(bool isSuccess, string? message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    protected Result(bool isSuccess, string? message, Error? error)
    {
        IsSuccess = isSuccess; 
        Message = message;
        Error = error;
    }

    protected Result(bool isSuccess,Error? error, Dictionary<string, string[]>? validationErrors, string? message)
    {
        IsSuccess = isSuccess; 
        Error = error;
        ValidationErrors = validationErrors;
        Message = message;
    }

    public static Result Success(string? message = null)
        => new Result(true, message);

    public static Result Failure(Error error)
        => new Result(false, error.Message, error);

    static IResult IResult.Failure(Error error)
        => new Result(false, error.Message, error);

    public static Result ValidationFailure(Error error,Dictionary<string, string[]> validationErrors, string? message)
        => new(false, error, validationErrors, message);

    public static implicit operator Result(Error error) => Failure(error);
    public static implicit operator Result(string? message = null) => Success(message);
}