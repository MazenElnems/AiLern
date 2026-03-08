namespace LMS.Application.Features.AssignmentSubmissions.Shared.DTO;

public class AssignmentSubmissionDto
{
    public int Id { get; set; }
    public DateTime SubmissionDate { get; set; }
    public int StudentId { get; set; }
    public int AssignmentId { get; set; }
    public bool IsLate { get; set; }
    public List<string> UploadFilesUrls { get; set; } = new();
}
