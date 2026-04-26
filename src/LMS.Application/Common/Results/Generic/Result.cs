using LMS.Domain.Errors;

namespace LMS.Application.Common.Results.Generic;

public class Result<T> : Result , IResult
{
    public T? Value { get; init; }

    private Result(bool isSuccess, T value, string? message)
        : base(isSuccess, message)
    {
        Value = value;
    }

    private Result(bool isSuccess, string? message, Error? error)
        : base(isSuccess, message, error)
    {

    }

    private Result(bool isSuccess, string? message, Error? error, Dictionary<string, string[]> validationErrors)
        : base(isSuccess, error, validationErrors, message)
    {

    }

    public static Result<T> Success(T value, string? message = null)
        => new(true, value, message);

    public static new Result<T> Failure(Error error)
        => new Result<T>(false, error.Message, error);

    static IResult IResult.Failure(Error error)
        => new Result<T>(false, error.Message, error);

    public static new Result<T> ValidationFailure(Error error, Dictionary<string, string[]> validationErrors, string? message)
    => new(false, message, error, validationErrors);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}

