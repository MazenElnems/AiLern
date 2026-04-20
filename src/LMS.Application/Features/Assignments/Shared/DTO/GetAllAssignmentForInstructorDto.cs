namespace LMS.Application.Features.Assignments.Shared.DTO;

public class GetAllAssignmentForInstructorDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPublished { get; set; }
    public bool AllowLateSubmission { get; set; }
    public int CourseId { get; set; }
}
