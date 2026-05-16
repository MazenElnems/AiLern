using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Users.Queries.GetUsersCount;

public class GetUsersCountQuery : IRequest<Result<GetUsersCountDto>>
{

}
