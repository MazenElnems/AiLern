using LMS.Domain.Common.Enums;

namespace LMS.Domain.Entities.Assignments;

public class AssignmentFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string StoragePath { get; set; }
    public UploadStatus UploadStatus { get; set; }

    // Foreign Keys
    public int AssignmentId { get; set; }

    // Navigation Properties
    public Assignment Assignment { get; set; }
}
