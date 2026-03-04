using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Auth.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.Login;

public class UserLoginByEmailAndPasswordCommand : IRequest<Result<GetTokenResponseDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
