using LMS.Domain.DTOs.MaterialFiles;

namespace LMS.Domain.DTOs.Sections;

public class CourseSectionsDto
{
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public string CourseName { get; set; }
    public List<MaterialFileMetadataDto> MaterialFiles { get; set; }
}
