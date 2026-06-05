using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.CourseDiscussion;

public class Discussion 
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AnswerAt { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }




    // Foreign Keys
    public int CourseId { get; set; }
    public int StudentId { get; set; }

    // Navigation Properties
    public Course Course { get; set; }
    public Student Student { get; set; }
    public List<DiscussionVote>? Votes { get; set; } = new List<DiscussionVote>();

}
