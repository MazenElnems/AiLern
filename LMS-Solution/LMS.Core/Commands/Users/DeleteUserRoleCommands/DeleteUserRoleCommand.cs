using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LMS.Core.Commands.Users.DeleteUserRoleCommands
{
    public class DeleteUserRoleCommand : IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string? Role { get; set; }
    }
}
