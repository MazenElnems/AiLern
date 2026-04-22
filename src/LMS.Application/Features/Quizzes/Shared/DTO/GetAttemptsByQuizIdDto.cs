using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetAttemptsByQuizIdDto
{
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; }
    public double TotalPoints { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public bool ShowResultOnClose { get; set; } 
    public List<AttemptMetaData> Attempts { get; set; } = new List<AttemptMetaData>();
}

public class AttemptMetaData
{
   public Guid Id { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public double? Score { get; set; }
    public int AttemptNumber { get; set; }
    public AttemptStatus Status { get; set; }
    public DateTime AttemptEndTime { get; set; }
}
