namespace LMS.Application.Features.Assignments.Shared.DTO;

public class AssignmentWithFilesDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public List<string> FileUrls { get; set; } = new(); 
}
