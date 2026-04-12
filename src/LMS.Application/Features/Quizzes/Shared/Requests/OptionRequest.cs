namespace LMS.Application.Features.Quizzes.Shared.Requests;

public class OptionRequest
{
    public Guid? OptionId { get; set; } 
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }
}
