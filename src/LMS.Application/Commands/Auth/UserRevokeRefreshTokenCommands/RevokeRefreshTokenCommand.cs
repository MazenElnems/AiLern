using MediatR;

namespace LMS.Application.Commands.Auth.UserRevokeRefreshTokenCommands;

public class RevokeRefreshTokenCommand : IRequest
{
    public string RefresToken { get; set; }
}
