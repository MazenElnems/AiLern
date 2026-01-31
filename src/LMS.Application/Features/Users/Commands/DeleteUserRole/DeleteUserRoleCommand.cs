using LMS.Application.Common.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Users.Commands.DeleteUserRole;

public class DeleteUserRoleCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Role { get; set; }
}