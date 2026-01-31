using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.RevokeToken;

public class RevokeRefreshTokenCommand : IRequest<Result>
{
    public string RefresToken { get; set; }
}
