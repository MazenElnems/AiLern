using LMS.Core.DTOs.Auth.Request;
using MediatR;

namespace LMS.Core.Commands.Auth.UserLoginCommands;

public class UserLoginByEmailAndPasswordCommand : IRequest<GetTokenResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
