namespace LMS.Application.Features.Courses.Shared.DTO;

public class AIResourcesDto
{
    public Guid FileId { get; set; }
    public string PresignedUrl { get; set; }
}
