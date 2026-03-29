using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(
        string FullName,
        string UserName,
        string Email,
        string Password,
        Roles Role,
        InstructorJobTitle? JobTitle
    ) : IRequest<Result>
{

}
