using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class AIQuestionGenerationJob
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public AIJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Error { get; set; }
    public string? HangfireJobId { get; set; }
}
