using LMS.Core.DTOs.Auth.Request;
using MediatR;

namespace LMS.Core.Commands.Auth.UserRefreshTokenCommands;

public class GetRefreshTokenCommand : IRequest<GetTokenResponseDto>
{
    public string RefreshToken { get; set; }
}
