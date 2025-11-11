namespace LMS.Core.CustomExceptions;

public class UnAuthorizedException : Exception
{
    public UnAuthorizedException(string message)
        : base(message)
    {
        
    }
}
