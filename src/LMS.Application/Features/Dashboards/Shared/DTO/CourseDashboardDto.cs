namespace LMS.Application.Features.Dashboards.Shared.DTO;

public class CourseDashboardDto 
{
    public int TotalEnrolledStudents { get; set; }
    public int TotalQuizzes { get; set; }
    public int TotalAssignments { get; set; }

    public List<QuizStatisticsDto> QuizStatistics { get; set; }
}
