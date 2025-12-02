using LMS.Core.Common;
using LMS.Core.DTOs.Users;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQuery : BasePagedQuery, IRequest<List<GetUsersByRoleDto>>
{
    [JsonIgnore]
    public int RoleId { get; set; }
}
