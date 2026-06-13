using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Report.Commands.CreateReport;

public class CreateMaterialReportCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid SectionId { get; set; }
    [JsonIgnore]
    public Guid MaterialId { get; set; }
    public string? Reason { get; set; }
    public ReportType ReportType { get; set; }

}
