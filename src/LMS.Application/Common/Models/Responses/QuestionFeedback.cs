namespace LMS.Application.Common.Models.Responses
{
    public class QuestionFeedback
    {
        public Guid Id { get; set; }
        public string Feedback { get; set; }
        public double EstimatedScore { get; set; } = 0;
    }
}