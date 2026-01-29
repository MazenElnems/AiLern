using MediatR;

namespace LMS.Application.Commands.Auth.ResendEmailConfirmationCommands;

public class ResendEmailConfirmationCommand : IRequest
{
    public string Email { get; set; }   
}
