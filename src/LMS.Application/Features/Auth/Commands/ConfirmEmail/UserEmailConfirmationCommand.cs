using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommand : IRequest<Result>
{
    public string Token { get; set; }
    public string Email { get; set; }
}
