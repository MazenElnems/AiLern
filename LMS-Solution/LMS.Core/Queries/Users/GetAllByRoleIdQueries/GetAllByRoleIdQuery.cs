using LMS.Core.Common;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Users;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQuery : BasePagedQuery, IRequest<PaginationResult<GetUsersByRoleDto>>
{
    [JsonIgnore]
    public int RoleId { get; set; }
}
