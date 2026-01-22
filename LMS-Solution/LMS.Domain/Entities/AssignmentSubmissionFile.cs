namespace LMS.Domain.Entities;

public class AssignmentSubmissionFile
{
    public int FileId { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; }

    // Foreign Keys
    public int AssignmentSubmissionId { get; set; }

    // Navigation Properties
    public AssignmentSubmission AssignmentSubmission { get; set; }
}
