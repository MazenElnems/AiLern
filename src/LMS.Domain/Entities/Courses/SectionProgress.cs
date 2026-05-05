using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.Courses;

public class SectionProgress
{
    public int StudentId { get; set; }
    public Guid SectionId { get; set; }
    public bool IsCompleted { get; set; }

    public Student Student { get; set; }
    public Section Section { get; set; }    
}
