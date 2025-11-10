namespace LMS.Core.CustomExceptions;

public class InvalidUserEmailOrPasswordException : Exception
{
    public InvalidUserEmailOrPasswordException(string message = "Invalid Email or Password")
        :base(message)
    {
        
    }
}
