namespace LMS.Application.Features.Quizzes.DTO;

public class QuizDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public bool ShowCorrectAnswersAfterClose { get; set; }
    public bool IsPublished { get; set; }
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    public int MaximumAttempts { get; set; }
    public int TotalPoints { get; set; }

}
