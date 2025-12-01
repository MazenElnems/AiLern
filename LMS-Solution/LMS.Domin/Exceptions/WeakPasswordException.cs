namespace LMS.Domin.Exceptions;

public class WeakPasswordException : Exception
{
    public WeakPasswordException(string message = "The provided password is too weak.") : base(message)
    {
    }
}
