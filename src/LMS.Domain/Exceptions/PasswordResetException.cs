namespace LMS.Domain.Exceptions;

public class PasswordResetException : Exception
{
    public PasswordResetException(string message = "can't reset the password") : base(message)
    {
    }
}
