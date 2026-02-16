using System;
using LMS.Domain.Common.Enums;

namespace LMS.Domain.Entities.Assignments;

public class AssignmentSubmissionFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string StoragePath { get; set; }
    public UploadStatus UploadStatus { get; set; }

    // Foreign Keys
    public int AssignmentSubmissionId { get; set; }

    // Navigation Properties
    public AssignmentSubmission AssignmentSubmission { get; set; }
}
