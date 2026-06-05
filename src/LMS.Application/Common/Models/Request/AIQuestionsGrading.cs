using LMS.Domain.Enums;

namespace LMS.Application.Common.Models.Request;

public class AIQuestionsGrading
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public double Mark { get; set; }
    public string? QuestionAnswer { get; set; }
    public List<InstructorCriterion>? InstructorCriteria { get; set; }
    public List<string>? Options { get; set; }
}
