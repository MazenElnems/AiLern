namespace LMS.Application.Features.Attempts.Shared.Requests;

public class SaveAttemptAnswerRequest
{
    public Guid QuestionId { get; set; }
    public string? WrittenAnswer { get; set; }  // for Written Question
    public Guid? OptionId { get; set; } // for MCQ or TrueFalse
}
