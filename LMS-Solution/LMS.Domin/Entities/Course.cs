using LMS.Domin.Enums;

namespace LMS.Domin.Entities;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public CourseStatus CourseStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    // Foreign Keys
    public int InstructorId { get; set; }

    // Navigation Properities
    public Instructor Instructor { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
