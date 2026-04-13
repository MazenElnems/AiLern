using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Application.Features.Auth.Shared.DTO;
using LMS.Application.Contracts.Identity;
using LMS.Application.Settings;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Auth.Commands.Login;

public class UserLoginByEmailAndPasswordCommandHandler : IRequestHandler<UserLoginByEmailAndPasswordCommand, Result<GetTokenResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly RefreshTokenOptions _refreshToken;
    private readonly JwtOptions _jwt;
    private readonly IUnitOfWork _unitOfWork;

    public UserLoginByEmailAndPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwt,
        IOptions<RefreshTokenOptions> refreshToken,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _refreshToken = refreshToken.Value;
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<GetTokenResponseDto>> Handle(UserLoginByEmailAndPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        if (!user.EmailConfirmed)
            return DomainErrors.Auth.EmailNotConfirmed;

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes);
        var accessToken = await _jwtTokenService.GenerateTokenAsync(user, accessTokenExpiration);
        var refreshToken = _refreshTokenService.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.InsertAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
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
            RefreshToken = refreshToken,
            Role = user.Role,
        };

        return Result<GetTokenResponseDto>.Success(response, "login successful");
    }
}

