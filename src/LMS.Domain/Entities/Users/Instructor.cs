using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Users;

public class Instructor : ApplicationUser
{
    public InstructorJobTitle JobTitle { get; set; }    

    // Navigation Properities
    public List<Course> Courses { get; set; } = new List<Course>();
}
