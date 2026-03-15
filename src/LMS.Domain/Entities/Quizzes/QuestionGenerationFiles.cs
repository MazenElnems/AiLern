namespace LMS.Domain.Entities.Quizzes;

public class QuestionGenerationFiles
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; }
    public bool HasUploadedToAIService { get; set; }
    public bool IsCourseMaterial { get; set; }

    // Foreign Key
    public Guid QuizId { get; set; }

    // Navigation Property
    public Quiz Quiz { get; set; }
}
