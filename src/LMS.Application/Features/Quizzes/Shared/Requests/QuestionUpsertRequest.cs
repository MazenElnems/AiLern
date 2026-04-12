using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.Requests;

public class QuestionUpsertRequest
{
    public Guid? Id { get; set; }
    public string QuestionText { get; set; }
    public QuestionType QuestionType { get; set; }
    public double Mark { get; set; }
    public string? Instructions { get; set; }
    public string? Explanation { get; set; }
    public List<OptionRequest> Options { get; set; } = new();
}
