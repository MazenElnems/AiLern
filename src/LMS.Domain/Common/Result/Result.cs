namespace LMS.Domain.Common.Result;

public class Result
{
    public bool Succeeded { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        Succeeded = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException());

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(T data) : base(true, null)
    {
        Data = data;
    }
    private Result(Error error) : base(false, error) { }

    public static Result<T> Success(T data) => new(data);
    public static new Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T data) => new(data);
    public static implicit operator Result<T>(Error error) => new(error);
}