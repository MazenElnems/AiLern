using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.CourseDiscussion;

public class DiscussionVote
{
    public Guid DiscussionId { get; set; }
    public int StudentId { get; set; }
    // Navigation Properties
    public Discussion Discussion { get; set; }
    public Student Student { get; set; }
}
