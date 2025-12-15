using LMS.Core.DTOs.Auth.Response;
using MediatR;

namespace LMS.Core.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommand : IRequest<EmailConfirmationResponse>
{
    public string Token { get; set; }
    public string Email { get; set; }
}
