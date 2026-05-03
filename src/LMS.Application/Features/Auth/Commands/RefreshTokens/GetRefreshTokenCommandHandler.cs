using LMS.Application.Common.Results.Generic;
using MediatR;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Application.Features.Auth.Shared.DTO;
using LMS.Application.Contracts.Identity;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Auth.Commands.RefreshTokens;

public class GetRefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IRefreshTokenService refreshTokenService,
    IJwtTokenService jwtTokenService) : IRequestHandler<GetRefreshTokenCommand, Result<GetTokenResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<Result<GetTokenResponseDto>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefreshToken, includeUser:true);

        if (oldRefreshToken == null)
            return DomainErrors.Auth.RefreshTokenNotFound(request.RefreshToken);

        // revoke old refresh token
        oldRefreshToken.RevokesOn = DateTime.UtcNow;

        var user = oldRefreshToken.User;

        var (accessToken, accessTokenExpiration) = await _jwtTokenService.GenerateTokenAsync(user);
        var (newRefreshToken, newRefreshTokenExpiration) = _refreshTokenService.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.InsertAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = newRefreshTokenExpiration,
            UserId = user.Id,
        });
        await _unitOfWork.CommitAsync(cancellationToken);

        GetTokenResponseDto response = new()
        {
            UserName = user.UserName!,
            Email = user.Email!,
            AccessToken = accessToken,
            ExpiresOn = accessTokenExpiration,
            RefreshToken = newRefreshToken,
            Role = user.Role
        };

        return response;
    }
}
