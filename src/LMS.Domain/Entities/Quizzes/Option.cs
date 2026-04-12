namespace LMS.Domain.Entities.Quizzes;

public class Option
{
    public Guid OptionId { get; set; }
    public int OptionNumber { get; set; }
    public string OptionText { get; set; }
    public bool IsCorrect { get; set; }

    // Foreign Keys
    public Guid QuestionId { get; set; }

    // Navigation Properties
    public Question Question { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new List<Answer>();
}
