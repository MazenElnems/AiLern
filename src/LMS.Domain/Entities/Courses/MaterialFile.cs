using LMS.Domain.Common.Enums;

namespace LMS.Domain.Entities.Courses;

public class MaterialFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public UploadStatus UploadStatus { get; set; }
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
    public string StoragePath { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }

    // Foreign Keys
    public Guid SectionId { get; set; }

    // Navigation Properties
    public Section Section { get; set; }
}
