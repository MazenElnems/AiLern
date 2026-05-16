namespace LMS.Application.Features.Instructors.Shared.DTO;

public class TopThreeCourseProgressDto
{
    public string CourseName { get; set; } = null!;
    public double ProgressPercentage { get; set; }
    public int StudentsCount { get; set; }
    public int QuizzesCount { get; set; }

}
