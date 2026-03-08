using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class Question
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public double Mark { get; set; }
    public int Order { get; set; }
    public string? Instructions { get; set; }
    public string? Explanation { get; set; } 

    // Foreign Keys
    public Guid QuizId { get; set; }

    // Navigation Properties
    public Quiz Quiz { get; set; }
    public List<Option> Options { get; set; } = new();
}
