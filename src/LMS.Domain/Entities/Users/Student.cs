using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Domain.Entities.Users;

public class Student : ApplicationUser
{
    // Navigation Properities
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public List<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
    public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    public List<CourseProgress> Progresses { get; set; } = new();
    public List<SectionProgress> SectionProgresses { get; set; } = new();
}
