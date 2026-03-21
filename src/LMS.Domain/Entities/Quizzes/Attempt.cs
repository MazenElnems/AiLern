using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class Attempt
{
    public Guid Id { get; set; }
    public int StudentId { get; set; }
    public Guid QuizId { get; set; }
    public DateTime AttemptEndTime { get; set; }
    public int? TimeSpent => SubmittedAt.HasValue ? (int)(SubmittedAt.Value - StartAt).TotalSeconds : null;
    public DateTime StartAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? SavedAt { get; set; }
    public double? Score => AttemptAnswers.Sum(a => a.Mark ?? 0);
    public int AttemptNumber { get; set; }
    public AttemptStatus Status { get; set; }

    // Navigation Properties
    public Student Student { get; set; } = null!;
    public Quiz Quiz { get; set; } = null!;
    public List<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
