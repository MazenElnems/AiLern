using LMS.Domain.Enums;

namespace LMS.Infrastructure.ExternalServices.AIService.Models;

public class AIQuestionGeneratedResponse
{
    public string Question { get; set; }
    public QuestionType QuestionType { get; set; }
    public QuestionDifficultyLevels Difficulty { get; set; }
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Answer { get; set; }
    public string Explaination { get; set; }
}
