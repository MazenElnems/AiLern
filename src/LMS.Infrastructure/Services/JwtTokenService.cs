using LMS.Application.Contracts.Identity;
using LMS.Domain.Entities.Users;
using LMS.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMS.Infrastructure.Services;

internal class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwt;

    public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwt)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
    }

    public async Task<(string, DateTime)> GenerateTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }
        .Union(roleClaims)
        .Union(userClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expirationDate = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials
        );

        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return (token, expirationDate);
    }
}
