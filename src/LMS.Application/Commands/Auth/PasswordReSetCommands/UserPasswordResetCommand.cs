using MediatR;

namespace LMS.Application.Commands.Auth.PasswordReSetCommands;

public class UserPasswordResetCommand : IRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
