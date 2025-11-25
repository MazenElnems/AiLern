using LMS.Core.ConfigurationOptions;
using LMS.Core.CustomExceptions;
using LMS.Core.DTOs.Auth.Request;
using LMS.Core.Services.Auth.Interfaces;
using LMS.Domin.Entities;
using LMS.Domin.RepositoriesInterfaces;
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
    private readonly IUsersRepository _usersRepository;

    public UserLoginByEmailAndPasswordCommandHandler(IAuthService authService, ILogger<UserLoginByEmailAndPasswordCommandHandler> logger, UserManager<ApplicationUser> userManager, IOptions<RefreshTokenOptions> refreshToken, IUsersRepository usersRepository)
    {
        _authService = authService;
        _logger = logger;
        _userManager = userManager;
        _refreshToken = refreshToken.Value;
        _usersRepository = usersRepository;
    }

    public async Task<GetTokenResponseDto> Handle(UserLoginByEmailAndPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new InvalidUserEmailOrPasswordException();

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new InvalidUserEmailOrPasswordException();


            // Email Confirmation 

            //if(!user.EmailConfirmed)
            //{
            //    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //}

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

            await _usersRepository.AddRefreshToken(refreshTokenEntity);

            GetTokenResponseDto response = new GetTokenResponseDto
            {
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = accessToken,
                ExpiresOn = accessTokenExpiration,
                RefreshToken = refreshToken
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
