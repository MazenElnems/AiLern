using LMS.Core.ConfigurationOptions;
using LMS.Core.Services.Auth.Interfaces;
using LMS.Domin.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Core.Services.Auth;

internal class AuthService : IAuthService
{
    private readonly JwtOptions _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILogger<AuthService> logger, IOptions<JwtOptions> JwtOptions)
    {
        _logger = logger;
        _jwt = JwtOptions.Value;
    }

    public (string, DateTime) GetAccessTokenAsync(List<Claim> claims)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var tokenExpireDate = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: tokenExpireDate,
            signingCredentials: signingCredentials
        );

        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return (token, tokenExpireDate);
    }

    public string GetRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
