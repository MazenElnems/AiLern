namespace LMS.Domin.Exceptions;

public class EmailConfirmationException : Exception
{
    public EmailConfirmationException(string message)
        : base(message) 
    {
        
    }
}
