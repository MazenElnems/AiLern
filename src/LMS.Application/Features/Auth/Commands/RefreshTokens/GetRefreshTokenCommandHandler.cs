using LMS.Application.ConfigurationOptions;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LMS.Application.Features.Auth.Commands.Login;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Application.Features.Auth.DTO;

namespace LMS.Application.Features.Auth.Commands.RefreshTokens;

public class GetRefreshTokenCommandHandler : IRequestHandler<GetRefreshTokenCommand, Result<GetTokenResponseDto>>
{
    private readonly RefreshTokenOptions _refreshToken;
    private readonly JwtOptions _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;

    public GetRefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, ILogger<UserLoginByEmailAndPasswordCommandHandler> logger, ITokensService authService, IOptions<RefreshTokenOptions> refreshToken, IUnitOfWork unitOfWork, IRefreshTokenService refreshTokenService, IJwtTokenService jwtTokenService, IOptions<JwtOptions> jwt)
    {
        _refreshToken = refreshToken.Value;
        _unitOfWork = unitOfWork;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _jwt = jwt.Value;
    }

    public async Task<Result<GetTokenResponseDto>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefreshToken, includeUser:true);

        if (oldRefreshToken == null)
            return DomainErrors.Auth.RefreshTokenNotFound(request.RefreshToken);

        // revoke old refresh token
        oldRefreshToken.RevokesOn = DateTime.UtcNow;

        var user = oldRefreshToken.User;

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes);
        var accessToken = await _jwtTokenService.GenerateTokenAsync(user, accessTokenExpiration);
        var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.InsertAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays),
            UserId = user.Id,
        });
        await _unitOfWork.CommitAsync();

        GetTokenResponseDto response = new GetTokenResponseDto
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
