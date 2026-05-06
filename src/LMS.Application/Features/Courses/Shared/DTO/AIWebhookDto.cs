using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Shared.DTO;

public class AIWebhookDto
{
    public Guid ProjectId { get; set; }
    public AIStatus Status { get; set; }
    public string? Error { get; set; }
}
