namespace LMS.Application.Features.Attempts.Shared.DTO;

public class GradeSubmissionDto
{
    public Guid QuestionId { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }

}
