using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptResultDto
{
    public Guid AttemptId { get; set; }
    public int StudentId { get; set; }
    public AttemptStatus Status { get; set; }
    public string QuizName { get; set; }
    public Guid QuizId { get; set; }
    public List<AnswerDto> Answers { get; set; }
    public int TimeSpentInSeconds { get; set; }
    public double TotalScore { get; set; }
    public double AchievedScore { get; set; }
}
