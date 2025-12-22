using LMS.Core.CurrentUser;
using MediatR;

namespace LMS.Core.Commands.Auth.SendPasswordResetEmailCommands;

public class SendPasswordResetEmailCommand : IRequest
{
    public string Email { get; set; }
}
