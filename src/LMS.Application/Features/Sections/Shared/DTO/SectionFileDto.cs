namespace LMS.Application.Features.Sections.Shared.DTO;

public class SectionFileDto
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
}
