using MediatR;

namespace LMS.Core.Commands.Auth.PasswordReSetCommands;

public class UserPasswordResetCommand : IRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
