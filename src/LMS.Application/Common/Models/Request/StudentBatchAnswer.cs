namespace LMS.Application.Common.Models.Request;

public class StudentBatchAnswer
{
    public Guid AttemptId { get; set; }
    public List<StudentAttemptAnswers> Answers { get; set; }
}