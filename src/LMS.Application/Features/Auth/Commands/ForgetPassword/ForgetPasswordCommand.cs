using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public class ForgetPasswordCommand : IRequest<Result>
{
    public string Email { get; set; }
}
