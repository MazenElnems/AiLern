namespace LMS.Application.DTOs.AssignmentSubmissions;

public class StudentsAssignmentSubmissionsDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime SubmissionDate { get; set; }
    public bool IsLate { get; set; }
}
