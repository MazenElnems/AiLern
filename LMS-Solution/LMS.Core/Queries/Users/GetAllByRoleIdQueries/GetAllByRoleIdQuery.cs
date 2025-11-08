using LMS.Core.Constants;
using LMS.Core.DTOs.Users;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries;

public class GetAllByRoleIdQuery : IRequest<List<GetUsersByRoleDto>>
{
    [JsonIgnore]
    public int RoleId { get; set; }
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
