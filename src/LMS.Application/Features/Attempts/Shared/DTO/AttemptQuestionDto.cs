using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptQuestionDto
{
    public Guid Id { get; set; }
    public string Question { get; set; }
    public QuestionType Type { get; set; }
    public double Mark { get; set; }   
    public string? Instructions { get; set; }
    public List<AttemptOptionDto>? Options { get; set; }
    public int Order { get; set; }  
    public string? WrittenAnswer { get; set; }
    public Guid? SelectedOptionId { get; set; }
    public List<Guid> ShuffledOptionIds { get; set; }
}
