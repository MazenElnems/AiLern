using LMS.Domain.Entities.Courses;

namespace LMS.Domain.Entities.Assignments;

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }

    // Foreign Keys
    public int CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; }
    public List<AssignmentFile> Files { get; set; } = new List<AssignmentFile>();
    public List<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
}
