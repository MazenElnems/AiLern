namespace LMS.Domain.Entities.Quizzes;

public class AttemptAnswer
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string? BooleanAnswer { get; set; }
    public string? WrittenAnswer { get; set; }
    public int? OptionNumber { get; set; }
    public double? Mark { get; set; }
    public string? Feedback { get; set; }

    // Navigation Properties
    public Attempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
