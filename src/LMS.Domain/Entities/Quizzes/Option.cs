namespace LMS.Domain.Entities.Quizzes;

public class Option
{
    public int OptionNumber { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }

    // Foreign Keys
    public Guid QuestionId { get; set; }

    // Navigation Properties
    public Question Question { get; set; }
}
