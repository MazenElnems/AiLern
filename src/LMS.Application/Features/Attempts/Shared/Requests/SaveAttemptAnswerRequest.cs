namespace LMS.Application.Features.Attempts.Shared.Requests;

public class SaveAttemptAnswerRequest
{
    public Guid QuestionId { get; set; }
    public string? BooleanAnswer { get; set; }
    public string? WrittenAnswer { get; set; }
    public int? OptionNumber { get; set; }
}
