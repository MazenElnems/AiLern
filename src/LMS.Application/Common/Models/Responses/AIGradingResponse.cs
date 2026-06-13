namespace LMS.Application.Common.Models.Responses;

public class AIGradingResponse
{
    public Guid AttemptId { get; set; }
    public List<AIAnswerFeedback> AnswersFeedback { get; set; } = new();
    public List<string> WeakTopics { get; set; } = new();
}
