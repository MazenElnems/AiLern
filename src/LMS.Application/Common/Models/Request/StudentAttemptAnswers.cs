namespace LMS.Application.Common.Models.Request;

public class StudentAttemptAnswers
{
    public Guid QuestionId { get; set; }
    public string StudentAnswer { get; set; }
}