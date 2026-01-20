namespace LMS.Domain.Exceptions;

public class CourseUpdateException : Exception
{
    public CourseUpdateException(string message)
        : base(message)
    {

    }
}
