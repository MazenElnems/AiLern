using LMS.Domain.Common;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Users;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQuery : BasePagedQuery, IRequest<PaginationResult<GetUsersByRoleDto>>
{
    [JsonIgnore]
    public int RoleId { get; set; }
}
