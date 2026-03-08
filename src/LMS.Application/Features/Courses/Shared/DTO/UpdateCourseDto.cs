namespace LMS.Application.Features.Courses.Shared.DTO;

public class UpdateCourseDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
