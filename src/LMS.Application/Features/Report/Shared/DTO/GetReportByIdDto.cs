using LMS.Domain.Enums;

namespace LMS.Application.Features.Report.Shared.DTO;

public class GetReportByIdDto
{
    public Guid ReportId { get; set; }
    public ReportType ReportType { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? ReportComment { get; set; }
    public ReportStatus ReportStatus { get; set; }


    public int ReporterId { get; set; }
    public string ReporterName { get; set; }
    public string ReporterEmail { get; set; }


    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; }
    public string MaterialType { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int InstructorId { get; set; }
    public string InstructorName { get; set; }
    public string InstructorEmail { get; set; }
    public string PreviewMaterialUrl { get; set; }
}
