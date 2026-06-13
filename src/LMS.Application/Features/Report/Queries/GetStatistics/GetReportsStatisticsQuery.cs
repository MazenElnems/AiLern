using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Report.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Report.Queries.GetStatistics;

public class GetReportsStatisticsQuery : IRequest<Result<ReportsStatisticsDto>>
{

}
