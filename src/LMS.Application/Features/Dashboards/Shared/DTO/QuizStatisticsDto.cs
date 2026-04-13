namespace LMS.Application.Features.Dashboards.Shared.DTO;

public class QuizStatisticsDto
{
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; }
    public double? AverageScore { get; set; }
    public double? AverageTimeSpentInMinutes { get; set; }
}
