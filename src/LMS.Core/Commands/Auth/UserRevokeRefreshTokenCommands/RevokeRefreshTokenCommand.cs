using MediatR;

namespace LMS.Core.Commands.Auth.UserRevokeRefreshTokenCommands;

public class RevokeRefreshTokenCommand : IRequest
{
    public string RefresToken { get; set; }
}
