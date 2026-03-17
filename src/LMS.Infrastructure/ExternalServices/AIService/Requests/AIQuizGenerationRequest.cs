using LMS.Domain.Enums;
using System.Text.Json.Serialization;

namespace LMS.Infrastructure.ExternalServices.AIService.Requests;

public class AIQuizGenerationRequest
{
    public string[] ProjectIds { get; set; } = Array.Empty<string>();
    public int QuestionsNumber { get; set; }
    public Dictionary<QuestionType, int> QuestionsTypes { get; set; } = new();
    public Dictionary<QuestionDifficultyLevels, float> DifficultyLevels { get; set; } = new();
    [JsonIgnore]
    public string? Query { get; set; }
}