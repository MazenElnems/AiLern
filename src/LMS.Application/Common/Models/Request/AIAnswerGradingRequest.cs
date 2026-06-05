using LMS.Domain.Enums;

namespace LMS.Application.Common.Models.Request;

public class AIAnswerGradingRequest
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; }
    public double Mark { get; set; }
    public QuestionType Type { get; set; }
    public List<string>? Options { get; set; }
    public List<AIGradingCriteriaRequest> GradingCriteria { get; set; } = new();
    public string? StudentAnswer { get; set; }
    public string? QuestionAnswer { get; set; }  
}
