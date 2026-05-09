using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AnswerDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public string? Answer { get; set; } // for written answer
    public int Order { get; set; }
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public string Feedback { get; set; }
    public string? Instructions { get; set; }
    public string? Explanation { get; set; }
    public List<OptionAnswerDto>? Options { get; set; }
}
