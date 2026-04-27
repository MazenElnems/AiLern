using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ConfirmChangeEmail;

public class ConfirmChangeUserEmailCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string NewEmail { get; set; }
    public string Token { get; set; }
}
