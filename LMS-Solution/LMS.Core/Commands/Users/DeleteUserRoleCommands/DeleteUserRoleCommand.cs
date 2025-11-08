using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Core.Commands.Users.DeleteUserRoleCommands;

public class DeleteUserRoleCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    [Required]
    public string? Role { get; set; }
}