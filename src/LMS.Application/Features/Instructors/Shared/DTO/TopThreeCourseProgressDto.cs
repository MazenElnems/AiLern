namespace LMS.Application.Features.Instructors.Shared.DTO;

public class TopThreeCourseProgressDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } 
    public double ProgressPercentage { get; set; }
    public int StudentsCount { get; set; }
    public int QuizzesCount { get; set; }

}
