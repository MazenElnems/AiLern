namespace LMS.Application.DTOs.Sections;

public class SectionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public int CourseId { get; set; }

}
