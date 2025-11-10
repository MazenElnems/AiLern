using LMS.Core.Commands.Auth.UserLoginCommands;
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

namespace LMS.Core.Commands.Auth.UserRefreshTokenCommands;

public class GetRefreshTokenCommandHandler : IRequestHandler<GetRefreshTokenCommand, GetTokenResponseDto>
{
    private readonly ILogger<UserLoginByEmailAndPasswordCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthService _authService;
    private readonly RefreshTokenOptions _refreshToken;
    private readonly IUsersRepository _usersRepository;

    public GetRefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, ILogger<UserLoginByEmailAndPasswordCommandHandler> logger, IAuthService authService, IOptions<RefreshTokenOptions> refreshToken, IUsersRepository usersRepository)
    {
        _logger = logger;
        _authService = authService;
        _refreshToken = refreshToken.Value;
        _usersRepository = usersRepository;
        _userManager = userManager;
    }

    public async Task<GetTokenResponseDto> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshToken = await _usersRepository.GetRefreshTokenAsync(request.RefreshToken)
            ?? throw new ResourceNotFoundException(nameof(RefreshToken), request.RefreshToken);

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
        await _usersRepository.CommitAsync();

        var newRefreshToken = _authService.GetRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
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
            RefreshToken = newRefreshToken
        };

        return response;
    }
}
