using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetSubmissionsByQuizIdDto
{
    public Guid Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int? TimeSpent { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public double? Score { get; set; }
    public int AttemptNumber { get; set; }
    public AttemptStatus Status { get; set; }

}
