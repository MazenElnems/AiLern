namespace LMS.Domain.Entities;

public class Instructor : ApplicationUser
{
    // Navigation Properities
    public List<Course> Courses { get; set; } = new List<Course>();
}
