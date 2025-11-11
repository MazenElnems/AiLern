using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Core.Commands.Users.DeleteUserRoleCommands;

public class DeleteUserRoleCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Role { get; set; }
}