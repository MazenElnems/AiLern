using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Queries.GetMyLearning;

public class GetMyLearningDto
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Percent { get; set; }
    public Guid? LastLearningItemId { get; set; }
    public int? LastPageNumber { get; set; }
    public int? LastWatchedTime { get; set; }
    public LearningType Type { get; set; }
}
