namespace LMS.Domain.Exceptions;

public class AIServiceTimeoutException : Exception
{
    public AIServiceTimeoutException(string message, Exception exception = null)
        : base(message, exception)
    {
        
    }
}
