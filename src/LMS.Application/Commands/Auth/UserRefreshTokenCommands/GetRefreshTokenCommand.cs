using LMS.Application.DTOs.Auth.Request;
using MediatR;

namespace LMS.Application.Commands.Auth.UserRefreshTokenCommands;

public class GetRefreshTokenCommand : IRequest<GetTokenResponseDto>
{
    public string RefreshToken { get; set; }
}
