using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Users.Queries.GetAllByRoleId;

public record GetAllByRoleQuery(Roles Role, int PageNo, int PageSize) : IRequest<Result<PaginationResult<GetUsersByRoleDto>>>
{ }
