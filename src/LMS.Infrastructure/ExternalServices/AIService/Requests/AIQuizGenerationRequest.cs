using LMS.Domain.Enums;

namespace LMS.Infrastructure.ExternalServices.AIService.Requests;

public class AIQuizGenerationRequest
{
    public string[] ProjectIDs { get; set; } = Array.Empty<string>();
    public int NumberOfQuestions { get; set; }
    public Dictionary<QuestionType, int> QuestionTypeCount { get; set; } = new Dictionary<QuestionType, int>();
    public Dictionary<QuestionDifficultyLevels, float> QuestionDifficultyPercents { get; set; } = new Dictionary<QuestionDifficultyLevels, float>();
    public string? Query { get; set; }
}
