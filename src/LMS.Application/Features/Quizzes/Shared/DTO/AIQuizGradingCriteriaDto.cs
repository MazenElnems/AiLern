namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class AIQuizGradingCriteriaDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; }
    public double Mark { get; set; }
    public string ModelAnswer { get; set; }
    public List<QuestionCriteriaDto> CriteriaList { get; set; }
}
