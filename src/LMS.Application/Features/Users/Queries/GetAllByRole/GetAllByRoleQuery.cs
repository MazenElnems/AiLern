using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Users.Queries.GetAllByRoleId;

public record GetAllByRoleQuery(int PageNo, int PageSize,Roles? Role = null) : IRequest<Result<PaginationResult<GetUsersByRoleDto>>>
{ }
