namespace LMS.Domin.DTOs.Courses;

public class GetCourseDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CourseStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
}
