namespace LMS.Domain.DTOs.Submission;

public class SubmissionDto
{
    public int Id { get; set; }
    public DateTime SubmissionDate { get; set; }

    public int StudentId { get; set; }
    public int AssignmentId { get; set; }
    public bool IsLate { get; set; }
}
