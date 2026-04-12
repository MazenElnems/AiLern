namespace LMS.Domain.Entities.Quizzes;

public class Answer
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string? WrittenAnswer { get; set; }
    public Guid? OptionId { get; set; }
    public double? Mark { get; set; }
    public string? Feedback { get; set; }

    // Navigation Properties
    public Attempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public Option? Option { get; set; }
}
