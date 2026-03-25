using LMS.Application.Features.Quizzes.Shared.Requests;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; }
    public string Type { get; set; }
    public string StudentAnswer { get; set; }
    public double? Score { get; set; }
    public double MaxScore { get; set; }
    public string Feedback { get; set; }
    public string? Instructions { get; set; }
    public string? Explanation { get; set; }
    public List<OptionDto>? Options { get; set; }
}
