namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class UpsertCriteriaDto
{
    public Guid? Id { get; set; }
    public string Criterion { get; set; }
    public double Mark { get; set; }    
}
