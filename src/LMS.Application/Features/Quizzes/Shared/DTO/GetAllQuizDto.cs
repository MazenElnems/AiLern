using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetAllQuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public QuizStatus Status { get; set; }
    public int MaximumAttempts { get; set; }
    public bool ShowResultOnClose { get; set; }
    public int AttemptTimeLimit { get; set; }   
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int QuestionsCount { get; set; }
    public int? StudentAttemptCount { get; set; }
    public bool HasActiveAttempt { get; set; }
}
