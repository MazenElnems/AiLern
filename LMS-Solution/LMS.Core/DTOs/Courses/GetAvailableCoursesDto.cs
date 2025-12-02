namespace LMS.Core.DTOs.Courses;

public class GetAvailableCoursesDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
}
