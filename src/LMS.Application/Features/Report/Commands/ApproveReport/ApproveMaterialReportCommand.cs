using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Report.Commands.ApproveReport;

public class ApproveMaterialReportCommand : IRequest<Result>
{
    public Guid ReportId { get; set; }

}
