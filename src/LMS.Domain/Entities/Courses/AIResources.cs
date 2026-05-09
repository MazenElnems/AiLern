using LMS.Domain.Enums;
using System.Net.NetworkInformation;

namespace LMS.Domain.Entities.Courses;

public class AIResource
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public double FileSize { get; set; }
    public string FileType { get; set; }
    public UploadStatus UploadStatus { get; set; }
    public AIStatus AIStatus { get; set; }
    public string StoragePath { get; set; }

    // Navigation property
    public Course Course { get; set; } 
    public int CourseId { get; set; }
}
