namespace LMS.Domain.Exceptions;

public class AIServiceUnAvailableException : Exception
{
    public AIServiceUnAvailableException(string message, Exception exception = null)
        : base(message, exception)
    {
        
    }
}
