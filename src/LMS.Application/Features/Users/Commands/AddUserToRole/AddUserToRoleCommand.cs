using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Users.Commands.AddUserToRole
{
    public class AddUserToRoleCommand :IRequest<Result>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
