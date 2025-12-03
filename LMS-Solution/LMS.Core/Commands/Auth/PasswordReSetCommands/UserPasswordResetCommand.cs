using MediatR;

namespace LMS.Core.Commands.Auth.PasswordReSetCommands;

public class UserPasswordResetCommand : IRequest
{
    public int UserId { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
