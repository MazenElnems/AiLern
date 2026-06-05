using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class QuizQuestionsWithCriteriaDto
{
    public Guid QuestionId { get; set; }
    public QuestionType QuestionType { get; set; }
    public string? ModelAnswer { get; set; }
    public Dictionary<string, double> Criteria { get; set; } = new();
    public List<string>? Options { get; set; }
}
