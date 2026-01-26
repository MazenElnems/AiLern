using System;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

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
