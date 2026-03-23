namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string StudentAnswer { get; set; }
    public string CorrectAnswer { get; set; }
    public double? Score { get; set; }
    public double MaxScore { get; set; }
    public string Feedback { get; set; }
}
