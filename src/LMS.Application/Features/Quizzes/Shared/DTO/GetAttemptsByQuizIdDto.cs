using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetAttemptsByQuizIdDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; }
    public int? TimeSpent { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public double? Score { get; set; }
    public int AttemptNumber { get; set; }
    public AttemptStatus Status { get; set; }

}
