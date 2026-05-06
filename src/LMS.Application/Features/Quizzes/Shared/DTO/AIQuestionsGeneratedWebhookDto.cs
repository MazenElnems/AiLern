using LMS.Application.Common.Models.Responses;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class AIQuestionsGeneratedWebhookDto
{
    [JsonPropertyName("question_type")]
    public string QuestionType { get; set; }
    [JsonPropertyName("generated_question")]
    public int GeneratedQuestions { get; set; }
    [JsonPropertyName("questions")]
    public List<AIQuestionGeneratedResponse> Questions { get; set; }
    [JsonPropertyName("completed")]
    public bool Completed { get; set; }
    [JsonPropertyName("quiz_id")]
    public Guid QuizId { get; set; }    
}
