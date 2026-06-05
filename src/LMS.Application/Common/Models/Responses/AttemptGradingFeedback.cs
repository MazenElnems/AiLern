namespace LMS.Application.Common.Models.Responses;

public class AttemptGradingFeedback
{
    public Guid AttemptId { get; set; }
    public List<string> WeakTopics { get; set; }
    public List<QuestionFeedback> QuestionFeedback { get; set; }
    public double SumScore { get; set; }    
}
