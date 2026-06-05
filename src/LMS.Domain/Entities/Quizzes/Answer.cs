using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class Answer
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string? WrittenAnswer { get; set; }
    public Guid? OptionId { get; set; }
    public double? Mark { get; set; }
    public string? Feedback { get; set; }
    public double? Confidence { get; set; }
    public List<Guid>? ShuffledOptionIds { get; set; } = new(); // JSON serialized list of option ids in the order they were presented to the student

    // Navigation Properties
    public Attempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public Option? Option { get; set; }
}
