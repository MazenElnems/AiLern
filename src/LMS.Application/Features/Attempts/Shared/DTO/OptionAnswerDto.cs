namespace LMS.Application.Features.Attempts.Shared.DTO;

public class OptionAnswerDto
{
    public int Order { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsSelected { get; set; }
}
