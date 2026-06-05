using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.Courses;

public class WeakTopic
{
    public int Id { get; set; } 
    public string Topic { get; set; }
    public Guid? AttemptId { get; set; }
    public int CourseId { get; set; }

    // Navigation Properties    
    public Attempt Attempt { get; set; }
    public Course Course { get; set; }
}
