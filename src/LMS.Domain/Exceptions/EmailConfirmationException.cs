namespace LMS.Domain.Exceptions;

public class EmailConfirmationException : Exception
{
    public EmailConfirmationException(string message)
        : base(message) 
    {
        
    }
}
