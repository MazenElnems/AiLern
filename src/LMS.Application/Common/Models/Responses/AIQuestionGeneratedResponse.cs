using LMS.Domain.Enums;
using System.Text.Json.Serialization;

namespace LMS.Application.Common.Models.Responses;

public class AIQuestionGeneratedResponse
{
    [JsonPropertyName("question")]
    public string Question { get; set; }
    [JsonPropertyName("question_type")]
    public QuestionType QuestionType { get; set; }
    [JsonPropertyName("difficulty")]
    public QuestionDifficultyLevels Difficulty { get; set; }
    [JsonPropertyName("options")]
    public List<string>? Options { get; set; }
    [JsonPropertyName("correct_answer")]
    public string? CorrectAnswer { get; set; }
    [JsonPropertyName("answer")]
    public string? Answer { get; set; }
    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}
