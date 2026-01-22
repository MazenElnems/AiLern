namespace LMS.Domain.Entities;

public class AssignmentFile
{
    public int FileId { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; }

    // Foreign Keys
    public int AssignmentId { get; set; }

    // Navigation Properties
    public Assignment Assignment { get; set; }
}
