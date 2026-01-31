using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public class SendPasswordResetEmailCommand : IRequest<Result>
{
    public string Email { get; set; }
}
