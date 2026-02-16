using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Users;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Users.Queries.GetAllByRoleId;

public class GetAllByRoleIdQuery : BasePaginatedQuery, IRequest<Result<PaginationResult<GetUsersByRoleDto>>>
{
    [JsonIgnore]
    public int RoleId { get; set; }
}
