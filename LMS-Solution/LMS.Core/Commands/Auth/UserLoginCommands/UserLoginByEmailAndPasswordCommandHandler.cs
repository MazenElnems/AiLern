using LMS.Core.ConfigurationOptions;
using LMS.Core.DTOs.Auth.Request;
using LMS.Core.Services.Auth.Interfaces;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LMS.Core.Commands.Auth.UserLoginCommands;

public class UserLoginByEmailAndPasswordCommandHandler : IRequestHandler<UserLoginByEmailAndPasswordCommand, GetTokenResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserLoginByEmailAndPasswordCommandHandler> _logger;
    private readonly IAuthService _authService;
    private readonly RefreshTokenOptions _refreshToken;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailSender _emailSender;
    private readonly ApplicationDomain _applicationDomain;

    public UserLoginByEmailAndPasswordCommandHandler(IAuthService authService,
        ILogger<UserLoginByEmailAndPasswordCommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        IOptions<RefreshTokenOptions> refreshToken,
        IUnitOfWork unitOfWork, 
        IMailSender emailSender,
        IOptions<ApplicationDomain> applicationDomainOptions)
    {
        _authService = authService;
        _logger = logger;
        _userManager = userManager;
        _refreshToken = refreshToken.Value;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _applicationDomain = applicationDomainOptions.Value;
    }

    public async Task<GetTokenResponseDto> Handle(UserLoginByEmailAndPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new InvalidUserEmailOrPasswordException();

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new InvalidUserEmailOrPasswordException();


            // Is Email Confirmed 

            if (!user.EmailConfirmed)
            {
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var template = await File.ReadAllTextAsync("EmailTemplates\\ConfirmationEmail.html");
                var html = template
                    .Replace("{{ConfirmationLink}}", $"{_applicationDomain.Domain}/api/auth/email-confirm?token={emailConfirmationToken}&email={user.Email}");

                await _emailSender.SendAsync(request.Email, "Email Confirmation", html);
                return new GetTokenResponseDto();
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

            return response;
        }
        catch(InvalidUserEmailOrPasswordException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception was thrown while user login");
            throw;
        }
    }
}
