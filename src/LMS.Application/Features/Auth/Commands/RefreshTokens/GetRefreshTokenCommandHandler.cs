using LMS.Application.ConfigurationOptions;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LMS.Application.Features.Auth.Commands.Login;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Application.Features.Auth.DTO;

namespace LMS.Application.Features.Auth.Commands.RefreshTokens;

public class GetRefreshTokenCommandHandler : IRequestHandler<GetRefreshTokenCommand, Result<GetTokenResponseDto>>
{
    private readonly ILogger<UserLoginByEmailAndPasswordCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokensService _authService;
    private readonly RefreshTokenOptions _refreshToken;
    private readonly IUnitOfWork _unitOfWork;

    public GetRefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, ILogger<UserLoginByEmailAndPasswordCommandHandler> logger, ITokensService authService, IOptions<RefreshTokenOptions> refreshToken, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _authService = authService;
        _refreshToken = refreshToken.Value;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<Result<GetTokenResponseDto>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshToken = await _unitOfWork.Users.GetRefreshTokenAsync(request.RefreshToken, includeUser:true)
            ;
        if (oldRefreshToken == null)
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.RefreshTokenNotFound(request.RefreshToken));

        var user = oldRefreshToken.User;

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Union(roleClaims).ToList();

        var (accessToken, accessTokenExpiration) = _authService.GetAccessTokenAsync(claims);

        oldRefreshToken.RevokesOn = DateTime.UtcNow;

        var newRefreshToken = _authService.GetRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays),
            UserId = user.Id,
        };

        await _unitOfWork.RefreshTokens.InsertAsync(refreshTokenEntity);
        await _unitOfWork.CommitAsync();

        GetTokenResponseDto response = new GetTokenResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresOn = accessTokenExpiration,
            RefreshToken = newRefreshToken,
            Role = roles.FirstOrDefault() ?? "Student"
        };

        return Result<GetTokenResponseDto>.Success(response);
    }
}
