namespace LMS.Domain.Entities;

public class Section
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int SectionNumber { get; set; }

    // Foreign Keys
    public int CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; } 
    public List<MaterialFile> MaterialFiles { get; set; } = new List<MaterialFile>();
}