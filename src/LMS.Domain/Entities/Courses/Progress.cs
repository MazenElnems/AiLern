using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Courses;

public class Progress 
{
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime UpdatedAt { get; set; }
    public double Percent { get; set; }
    public int? LastPageNumber { get; set; }
    public int? LastWatchedTime { get; set; }
    public LearningType Type { get; set; }  
    public Guid? LastLearningItemId { get; set; }

    public Course Course { get; set; }
    public Student Student { get; set; }
}
