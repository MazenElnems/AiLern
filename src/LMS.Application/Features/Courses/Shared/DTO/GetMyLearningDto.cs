using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Shared.DTO;

public class GetMyLearningDto
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? LastLearningItemId { get; set; }
    public int? LastPageNumber { get; set; }
    public int? LastWatchedTime { get; set; }
    public LearningType Type { get; set; }
}
