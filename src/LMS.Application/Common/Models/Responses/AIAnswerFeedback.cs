namespace LMS.Application.Common.Models.Responses;

public class AIAnswerFeedback
{
    public Guid Id { get; set; }
    public string Feedback { get; set; }
    public double EstinatedScore { get; set; }  
}
