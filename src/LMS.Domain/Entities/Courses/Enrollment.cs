using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.Courses;

public class Enrollment
{
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public DateTime EnrolledAt { get; set; }

    // Navigation Properities
    public Student Student { get; set; }
    public Course Course { get; set; }
}
