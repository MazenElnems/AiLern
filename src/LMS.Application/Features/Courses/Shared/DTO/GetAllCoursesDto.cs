namespace LMS.Application.Features.Courses.Shared.DTO;

public class GetAllCoursesDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
}
