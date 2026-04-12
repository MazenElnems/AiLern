namespace LMS.Application.Features.Quizzes.Shared.Requests;

public class OptionDto
{
    public int OptionNumber { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsSelected { get; set; }
}
