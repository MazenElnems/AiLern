using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Quizzes;

public class Attempt
{
    public Guid Id { get; set; }
    public int StudentId { get; set; }
    public Guid QuizId { get; set; }
    public DateTime AttemptEndTime { get; set; }
    public int? TimeSpent => SubmittedAt.HasValue ? (int)(SubmittedAt.Value - StartAt).TotalMinutes : null;
    public DateTime StartAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? SavedAt { get; set; }
    public double? Score => Answers.Sum(a => a.Mark ?? 0);
    public int AttemptNumber { get; set; }
    public AttemptStatus Status { get; set; }
    public List<Guid>? ShuffledQuestionIds { get; set; }     // JSON serialized list of question ids in the order they were presented to the student
    public string AutoSubmitJobId { get; set; } 

    // Navigation Properties
    public Student Student { get; set; } = null!;
    public Quiz Quiz { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new List<Answer>();

    public void Submit()
    {
        Status = AttemptStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public static Attempt StartNew(
        int studentId,
        int attemptNumber,
        DateTime startAt,
        DateTime attemptEndTime,
        IEnumerable<KeyValuePair<Guid, List<Guid>>> questionIdsWithOptionIds,
        bool shuffleQuestions = false,
        bool shuffleOptions = false)
    {
        var attempt = new Attempt
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            Status = AttemptStatus.InProgress,
            AttemptEndTime = attemptEndTime,
            StartAt = startAt,
            AttemptNumber = attemptNumber,
        };

        if (shuffleQuestions)
            attempt.ShuffledQuestionIds = questionIdsWithOptionIds.Select(q => q.Key).OrderBy(q => Guid.NewGuid()).ToList();

        foreach(var question in questionIdsWithOptionIds)
        {
            attempt.Answers.Add(new Answer
            {
                QuestionId = question.Key,
                ShuffledOptionIds = shuffleOptions ? question.Value.OrderBy(o => Guid.NewGuid()).ToList() : null,
                Mark = 0
            });
        }

        return attempt;
    }
}

