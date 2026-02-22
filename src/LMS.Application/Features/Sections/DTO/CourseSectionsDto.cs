namespace LMS.Application.Features.Sections.DTO;

public class CourseSectionsDto
{
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public string CourseName { get; set; }
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
    public List<SectionFileDto> SectionFiles { get; set; } = new();
}
