using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Instructors.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Instructors.Queries.GetMyDashboard;

public class GetInstructorDashboardQuery : IRequest<Result<InstructorDashboardDto>>
{
}
