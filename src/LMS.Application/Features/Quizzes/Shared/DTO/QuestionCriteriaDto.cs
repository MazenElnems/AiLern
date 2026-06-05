namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class QuestionCriteriaDto
{
    public Guid? Id { get; set; }    
    public string Criteria { get; set; }
    public int Mark { get; set; }   
}
