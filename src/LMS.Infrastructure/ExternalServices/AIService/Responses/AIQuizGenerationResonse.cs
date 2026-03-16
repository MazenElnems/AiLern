using System.Text.Json.Serialization;

namespace LMS.Infrastructure.ExternalServices.AIService.Responses;

public class AIQuizGenerationResonse
{
    public List<AIQuestionGeneratedResponse> Questions { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public int Total { get; set; }
}
