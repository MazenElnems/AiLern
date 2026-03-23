using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class GetAttemptByIdDto
{
    public int StudentId { get; set; }
    public AttemptStatus Status { get; set; }
    public string QuizName { get; set; }
    public Guid QuizId { get; set; }
    public List<AttemptResultDto> AttemptResult { get; set; }
    public double TotalScore { get; set; }
    public double? AchievedScore { get; set; }


}
