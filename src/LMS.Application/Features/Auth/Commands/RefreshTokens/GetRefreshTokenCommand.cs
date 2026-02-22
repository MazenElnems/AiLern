using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Auth.DTO;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.RefreshTokens;

public class GetRefreshTokenCommand : IRequest<Result<GetTokenResponseDto>>
{
    public string RefreshToken { get; set; }
}
