using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public int MaximumAttempts { get; set; }
    public bool ShowResultOnClose { get; set; }
    public double TotalPoints => Questions.Sum(q => q.Mark);
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    public QuizStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Foreign Keys
    public int CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; }
    public List<Question> Questions { get; set; } = new();
    public List<AIQuestionGenerationJob> QuestionGenerationJobs { get; set; } = new();  
}
