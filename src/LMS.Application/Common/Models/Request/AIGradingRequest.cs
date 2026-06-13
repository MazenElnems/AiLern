namespace LMS.Application.Common.Models.Request;

public class AIGradingRequest
{
    public List<AIQuestionsGrading> Questions { get; set; }
    public List<StudentBatchAnswer> StudentAnswers { get; set; }
}
