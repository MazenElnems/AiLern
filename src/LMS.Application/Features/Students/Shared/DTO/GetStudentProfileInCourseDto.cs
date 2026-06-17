using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;

namespace LMS.Application.Features.Students.Shared.DTO;

public class GetStudentProfileInCourseDto
{
    public List<SubmissionProfileDto> Assignments { get; set; }
    public List<QuizProfileDto> Quizzes { get; set; }
    public double? AverageQuizzesScore { get; set; }
    public int Progress { get; set; }
}
public class SubmissionProfileDto
{
    public string AssignmentName { get; set; }
    public int AssignmentId { get; set; }
    public int SubmissionId { get; set; }
    public List<MySubmissionFilesDto> SubmissionFiles { get; set; }
    public string? SubmissionFeedback { get; set; }
}
public class QuizProfileDto
{
    public string QuizName { get; set; }
    public Guid QuizId { get; set; }
    public double TotalPoints { get; set; }    
    public List<AttemptProfileDto> Attempts { get; set; }

}
public class AttemptProfileDto
{
    public Guid AttemptId { get; set; }
    public int AttemptNumber { get; set; }
    public double? Score { get; set; }
    public DateTime? SubmittedAt { get; set; }

}
