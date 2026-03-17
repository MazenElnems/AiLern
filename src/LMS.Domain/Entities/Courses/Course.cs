using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Courses;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Foreign Keys
    public int InstructorId { get; set; }

    // Navigation Properities
    public Instructor Instructor { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new();
    public List<Assignment> Assignments { get; set; } = new();
    public List<Section> Sections { get; set; } = new();
    public List<Quiz> Quizzes { get; set; } = new();
}
