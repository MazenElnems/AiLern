namespace LMS.Application.Features.Report.Shared.DTO;

public class GetAllReportsDto
{
    public Guid ReportId { get; set; }
    public string Material { get; set; }
    public string Reason { get; set; }
    public string Reporter { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }

}
