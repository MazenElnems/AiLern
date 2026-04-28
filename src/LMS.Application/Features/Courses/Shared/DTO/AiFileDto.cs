namespace LMS.Application.Features.Courses.Shared.DTO;

public class AiFileDto
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public double FileSize { get; set; }
    public string ContentType { get; set; }
}
