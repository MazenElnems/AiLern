using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Shared.DTO;

public class AiFileDto
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public double FileSize { get; set; }
    public string ContentType { get; set; }
    public AIStatus AIStatus { get; set; }
    public UploadStatus UploadStatus { get; set; }
}
