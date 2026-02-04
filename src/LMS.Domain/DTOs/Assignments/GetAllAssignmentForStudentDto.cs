namespace LMS.Domain.DTOs.Assignments;

public class GetAllAssignmentForStudentDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
    public bool IsSubmitted { get; set; }
    public bool IsLate => SubmissionDate > DueDate;
    public DateTime? SubmissionDate { get; set; }
    public int CourseId { get; set; }
}
