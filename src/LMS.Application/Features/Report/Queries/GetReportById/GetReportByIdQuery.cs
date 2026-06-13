using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Report.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Report.Queries.GetReportById;

public record GetReportByIdQuery(Guid ReportId) : IRequest<Result<GetReportByIdDto>>
{
}
