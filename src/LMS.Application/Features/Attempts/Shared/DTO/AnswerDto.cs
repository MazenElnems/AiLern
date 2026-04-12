using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AnswerDto
{
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public string? Answer { get; set; } // for written answer
    public double Score { get; set; }
    public double MaxScore { get; set; }
    public string Feedback { get; set; }
    public string? Instructions { get; set; }
    public string? Explanation { get; set; }
    public List<OptionDto>? Options { get; set; }
}


