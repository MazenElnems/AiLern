using LMS.Domain.Enums;

namespace LMS.Application.Common.Models.Request;

public class AIQuizGenerationRequest
{
    public List<string> ProjectIds { get; set; } = new();
    public int QuestionsNumber { get; set; }
    public Dictionary<QuestionType, int> QuestionsTypes { get; set; } = new();
    public Dictionary<QuestionDifficultyLevels, float> DifficultyLevels { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public string? Query { get; set; }
    public Guid QuizId { get; set; }    
}