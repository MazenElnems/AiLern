namespace LMS.Domin.Exceptions;

public class UserCreationException : Exception
{
    public UserCreationException(string message = "this user already exists")
        : base(message)
    {
    }
}
