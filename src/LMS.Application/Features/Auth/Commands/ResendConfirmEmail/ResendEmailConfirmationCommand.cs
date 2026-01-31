using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ResendConfirmEmail;

public class ResendEmailConfirmationCommand : IRequest<Result>
{
    public string Email { get; set; }   
}
