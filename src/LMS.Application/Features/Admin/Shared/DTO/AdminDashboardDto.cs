namespace LMS.Application.Features.Admin.Shared.DTO;

public class AdminDashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }

    public List<TopCourseDto> TopCourses { get; set; }

    public List<UserGrowthPerMonthDto> UserGrowthPerMonths { get; set; }
}


public class UserGrowthPerMonthDto
{
    public string Month { get; set; }
    public int StudentsCount { get; set; }
    public int InstructorsCount { get; set; }

    public double StudentsGrowthPercentage { get; set; }
    public double InstructorsGrowthPercentage { get; set; }
}

public class TopCourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string InstructorName { get; set; }
    public int TotalStudents { get; set; }
}