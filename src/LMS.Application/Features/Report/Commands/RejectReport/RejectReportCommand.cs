using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Report.Commands.RejectReport;

public record RejectReportCommand(Guid ReportId) : IRequest<Result>
{
}
