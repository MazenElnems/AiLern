using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ChangeUserEmail;

public class ChangeUserEmailCommand : IRequest<Result>
{
    public string NewEmail { get; set; } 
    public string CurrentPassword { get; set; }
}
