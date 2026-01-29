using LMS.Application.CurrentUser;
using MediatR;

namespace LMS.Application.Commands.Auth.SendPasswordResetEmailCommands;

public class SendPasswordResetEmailCommand : IRequest
{
    public string Email { get; set; }
}
