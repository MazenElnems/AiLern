namespace LMS.Application.Features.AssignmentSubmissions.Shared.DTO;

public class MySubmissionDto
{
    public int SubmissionId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public int StudentId { get; set; }
    public int AssignmentId { get; set; }
    public string? Feedback { get; set; }
    public bool IsLate { get; set; }
    public List<MySubmissionFilesDto> FilesUrls { get; set; } = new();
}

public class MySubmissionFilesDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FileUrl { get; set; }

}
