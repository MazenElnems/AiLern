namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class GetJobDto
{
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Error { get; set; }
    

}
