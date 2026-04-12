using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptQuestionDto
{
    public Guid Id { get; set; }
    public string Question { get; set; }
    public QuestionType Type { get; set; }
    public string? Instructions { get; set; }
    public List<AttemptOptionDto>? Options { get; set; }
    public string? WrittenAnswer { get; set; }
    public Guid? SelectedOptionId { get; set; }
}
