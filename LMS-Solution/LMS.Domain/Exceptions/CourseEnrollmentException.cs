namespace LMS.Domain.Exceptions;

public class CourseEnrollmentException : Exception
{
    public CourseEnrollmentException(string message)
        : base(message)
    {
        
    }
}
