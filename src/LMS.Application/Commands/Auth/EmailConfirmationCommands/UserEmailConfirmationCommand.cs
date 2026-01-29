using MediatR;

namespace LMS.Application.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommand : IRequest
{
    public string Token { get; set; }
    public string Email { get; set; }
}
