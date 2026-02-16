namespace LMS.Application.DTOs.Courses;

public class GetStudentCoursesDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
}
