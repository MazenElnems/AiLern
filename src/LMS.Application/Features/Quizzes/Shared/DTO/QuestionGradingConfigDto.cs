namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class QuestionGradingConfigDto
{
    public string ModelAnswer { get; set; }
    public IEnumerable<UpsertCriteriaDto> Criteria { get; set; }     
}
