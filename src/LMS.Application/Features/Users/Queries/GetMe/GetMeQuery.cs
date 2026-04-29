using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Users.Queries.GetMe;

public class GetMeQuery : IRequest<Result<GetMeDto>>
{
}
