using MediatR;

namespace LMS.Core.Commands.Auth.ResendEmailConfirmationCommands;

public class ResendEmailConfirmationCommand : IRequest
{
    public string Email { get; set; }   
}
