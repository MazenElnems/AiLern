namespace LMS.Application.Common.Models.Request;

public class AIQuizSubmissionGradingRequest
{
    public Guid AttemptId { get; set; }
    public List<AIAnswerGradingRequest> StudentAnswers { get; set; } = new();
}
