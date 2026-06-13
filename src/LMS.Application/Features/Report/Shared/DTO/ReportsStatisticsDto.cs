using LMS.Domain.Enums;

namespace LMS.Application.Features.Report.Shared.DTO;

public class ReportsStatisticsDto
{
    public int TotalReports { get; set; }
    public int ApprovedReports { get; set; }
    public int UnderReviewReports { get; set; }
    public int RejectedReports { get; set; }
    public Dictionary<string, int> TopReportReasons { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> TopReportForMaterial { get; set; } = new Dictionary<string, int>();
}
