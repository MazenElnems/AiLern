using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Application.Features.Auth.Shared.DTO;
using LMS.Application.Contracts.Identity;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Auth.Commands.Login;

public class UserLoginByEmailAndPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService) : IRequestHandler<UserLoginByEmailAndPasswordCommand, Result<GetTokenResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<GetTokenResponseDto>> Handle(UserLoginByEmailAndPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        if (!user.EmailConfirmed)
            return DomainErrors.Auth.EmailNotConfirmed;

        var (accessToken, accessTokenExpiration) = await _jwtTokenService.GenerateTokenAsync(user);
        var (refreshToken, refreshTokenExpiration) = _refreshTokenService.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.InsertAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = refreshTokenExpiration,
            UserId = user.Id,
        });
        await _unitOfWork.CommitAsync(cancellationToken);

        GetTokenResponseDto response = new()
        {
            UserName = user.UserName!,
            Email = user.Email!,
            AccessToken = accessToken,
            ExpiresOn = accessTokenExpiration,
            RefreshToken = refreshToken,
            Role = user.Role
        };

        return Result<GetTokenResponseDto>.Success(response, "login successful");
    }
}
