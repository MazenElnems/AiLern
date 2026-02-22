namespace LMS.Application.Features.Sections.DTO;

public class SectionFileDto
{
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
}
