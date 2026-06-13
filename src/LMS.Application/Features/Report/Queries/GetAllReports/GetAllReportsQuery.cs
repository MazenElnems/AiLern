using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Application.Features.Report.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Report.Queries.GetAllReports;

public record GetAllReportsQuery(int PageNo,int PageSize,ReportType? Type=null) : IRequest<Result<PaginationResult<GetAllReportsDto>>>
{
}
