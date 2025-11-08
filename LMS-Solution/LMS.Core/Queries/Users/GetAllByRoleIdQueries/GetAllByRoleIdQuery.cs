using AutoMapper.Configuration.Annotations;
using LMS.Shared.DTOs.Users;
using LMS.Shared.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Users.GetAllByRoleIdQueries
{
    public class GetAllByRoleIdQuery/*(int RoleId)*/ : IRequest<List<GetUsersByRoleDto>>
    {
        [JsonIgnore]
        public int RoleId { get; set; }
        public string? SortBy { get; set; }
        public string? Order { get; set; } = SortOrderOptions.DESC;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
