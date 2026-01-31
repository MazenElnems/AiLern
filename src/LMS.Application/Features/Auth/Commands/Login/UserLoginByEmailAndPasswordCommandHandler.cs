using LMS.Application.ConfigurationOptions;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Auth.Request;
using LMS.Application.Services.Auth.Interfaces;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LMS.Application.Features.Auth.Commands.Login;

public class UserLoginByEmailAndPasswordCommandHandler : IRequestHandler<UserLoginByEmailAndPasswordCommand, Result<GetTokenResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokensService _authService;
    private readonly RefreshTokenOptions _refreshToken;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailSender _emailSender;
    private readonly ApplicationDomain _applicationDomain;

    public UserLoginByEmailAndPasswordCommandHandler(ITokensService authService,
        UserManager<ApplicationUser> userManager,
        IOptions<RefreshTokenOptions> refreshToken,
        IUnitOfWork unitOfWork, 
        IMailSender emailSender,
        IOptions<ApplicationDomain> applicationDomainOptions)
    {
        _authService = authService;
        _userManager = userManager;
        _refreshToken = refreshToken.Value;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _applicationDomain = applicationDomainOptions.Value;
    }

    public async Task<Result<GetTokenResponseDto>> Handle(UserLoginByEmailAndPasswordCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.InvalidCredentials);

        // Is Email Confirmed 

        if (!user.EmailConfirmed)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var template = await File.ReadAllTextAsync("EmailTemplates\\ConfirmationEmail.html");
            var html = template
                .Replace("{{ConfirmationLink}}", $"{_applicationDomain.Domain}/api/auth/email-confirm?token={emailConfirmationToken}&email={user.Email}");

            await _emailSender.SendAsync(request.Email, "Email Confirmation", html);
            return Result<GetTokenResponseDto>.Failure(DomainErrors.Auth.EmailNotConfirmed);
        }

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
        var refreshToken = _authService.GetRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
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
            RefreshToken = refreshToken,
            Role = roles.FirstOrDefault()
        };

        return Result<GetTokenResponseDto>.Success(response, "login successful");
    }
}
