namespace LMS.Domain.Entities;

public class AssignmentSubmission
{
    public int Id { get; set; }
    public DateTime SubmissionDate { get; set; }
    public bool IsLate { get; set; }
    public string? Feedback { get; set; }

    // Foreign Keys
    public int StudentId { get; set; }
    public int AssignmentId { get; set; }

    // Navigation Properties
    public Student Student { get; set; }
    public Assignment Assignment { get; set; }
    public List<AssignmentSubmissionFile> Files { get; set; } = new List<AssignmentSubmissionFile>();
}
