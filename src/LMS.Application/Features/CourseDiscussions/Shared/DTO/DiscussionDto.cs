namespace LMS.Application.Features.CourseDiscussions.Shared.DTO;

public class DiscussionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Question { get; set; }
    public string? Answer { get; set; }
    public DateTime? AnswerAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? InstructorName { get; set; }
    public string? InstructorAvatar { get; set; }
    public string StudentName { get; set; }
    public string? StudentAvatar { get; set; }
    public int VotesCount { get; set; } = 0;    
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }




}
