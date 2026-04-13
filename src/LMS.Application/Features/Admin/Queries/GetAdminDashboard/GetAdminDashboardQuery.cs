using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Admin.Shared.DTO;
using LMS.Domain.Entities.Users;
using MediatR;

namespace LMS.Application.Features.Admin.Queries.GetAdminDashboard;

public class GetAdminDashboardQuery : IRequest<Result<AdminDashboardDto>>
{

}