namespace LMS.Application.DTOs.Sections;

public class CourseSectionsDto
{
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public string CourseName { get; set; }
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
    public List<SectionFileDto> SectionFiles { get; set; } = new();
}
