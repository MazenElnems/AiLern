namespace LMS.Domain.Entities.Quizzes;

public class AIGradingCriteria
{
    public Guid Id { get; set; }
    public string Criteria { get; set; }
    public double Mark { get; set; }

    // Foreign Keys
    public Guid QuestionId { get; set; }
    
    // Navigation Properties
    public Question Question { get; set; } = null!;
}
