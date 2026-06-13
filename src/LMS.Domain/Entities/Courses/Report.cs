using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities.Courses;

public class Report
{
    public Guid Id { get; set; }
    public ReportType Type { get; set; }
    public string? Comment { get; set; }
    public ReportStatus Status { get; set; }

    public DateTime SubmittedAt { get; set; }

    // foreign key
    public Guid MaterialId { get; set; }
    public int StudentId { get; set; }

    // navigation Properties
    public MaterialFile MaterialFile { get; set; }
    public Student Student { get; set; }

}
