using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Commands.Users.UpdateCommands
{
    public class AddUserToRoleCommand :IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
