using LMS.Domain.Enums;

namespace LMS.Application.Common.Models.Responses;

public class AIQuestionGeneratedResponse
{
    public string Question { get; set; }
    public QuestionType QuestionType { get; set; }
    public QuestionDifficultyLevels Difficulty { get; set; }
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Answer { get; set; }
    public string? Explanation { get; set; }
}
