namespace LMS.Domain.Entities;

public class Student : ApplicationUser
{
    public int StudentId { get; set; }

    // Navigation Properities
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public List<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
}
