using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetQuizDto
{
    public Guid Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public int MaximumAttempts { get; set; }
    public double TotalPoints { get; set; } 

    // Doctor Only

    public bool? IsPublished { get; set; }
    public QuizStatus? Status { get; set; }
    public bool? ShowResultOnClose { get; set; }
    public bool? ShuffleQuestions { get; set; }
    public bool? ShuffleOptions { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<QuestionDto>? Questions { get; set; }
}
