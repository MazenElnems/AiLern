using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Dashboards.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Dashboards.Queries.CourseDashboard;

public record GetCourseDashboardQuery(
    int CourseId
) : IRequest<Result<CourseDashboardDto>>
{ }
