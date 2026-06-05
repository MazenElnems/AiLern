namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class OptionDto
{
    public Guid OptionId { get; set; }
    public int OptionNumber { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}
