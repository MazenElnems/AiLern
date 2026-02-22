using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Courses;

public class Enrollment
{
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }

    // Navigation Properities
    public Student Student { get; set; }
    public Course Course { get; set; }
}
