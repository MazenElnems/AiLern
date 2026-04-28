namespace LMS.Application.Features.Courses.Shared.DTO;

public class GetInstructorCoursesDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string CourseStatus { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InstructorId { get; set; }
    public int TotalStudents { get; set; }
    public int TotalSections { get; set; }
    public string? ImageUrl { get; set; }
}
