namespace LMS.Core.Models
{
    public class Result
    {
        public IEnumerable<string> Errors { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result Success(string message = "")
        {
            return new Result
            {
                IsSuccess = true,
                Message = message,
                Errors = Array.Empty<string>()
            };
        }

        public static Result Failure(IEnumerable<string> errors, string message = "")
        {
            return new Result
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class Result<T> : Result where T : class
    {
        public T Data { get; set; }

        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                Errors = Array.Empty<string>()
            };
        }

        public static Result<T> Failure(IEnumerable<string> errors, string message = "")
        {
            return new Result<T>
            {
                Data = null,
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
