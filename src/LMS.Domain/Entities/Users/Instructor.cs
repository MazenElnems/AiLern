using LMS.Domain.Entities.Courses;

namespace LMS.Domain.Entities.Users;

public class Instructor : ApplicationUser
{
    // Navigation Properities
    public List<Course> Courses { get; set; } = new List<Course>();
}
