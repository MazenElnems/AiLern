using LMS.API.ConfigurationOptions;
using LMS.Core.ConfigurationOptions;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Domain.Entities;
using LMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMS.Core.DTOs.Authentication;
using LMS.Core.Services.Authentication.Interfaces;

namespace LMS.Core.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwt;
        private readonly RefreshTokenSettings _refreshToken;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwt, IOptions<RefreshTokenSettings> refreshTokenOptions)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _refreshToken = refreshTokenOptions.Value;
        }

        public async Task<Result> CreateUserAsync(string adminUserName, RegisterDto registerDto)
        {
            if (adminUserName == null)
                return Result.Failure(["Unauthorized action"], message: "Registration failed");

            if (await _userManager.FindByNameAsync(registerDto.UserName) is not null)
                return Result.Failure(["The username is already taken"], message: "Registration failed");

            if (!(registerDto.Role == UserRoles.Student || registerDto.Role == UserRoles.Instructor || registerDto.Role == UserRoles.Admin))
                return Result.Failure(["The specified role is invalid"], message: "Registration failed");

            var user = new ApplicationUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                CreatedBy = adminUserName,
            };

            var userResult = await _userManager.CreateAsync(user, registerDto.Password);

            if (!userResult.Succeeded)
            {
                var errors = userResult.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors, message: "Registration failed");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var errors = roleResult.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors, message: "Registration failed");
            }

            return Result.Success("Account created successfully");
        }

        public async Task<Result<TokenModel>> GetTokenAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Result<TokenModel>.Failure(["Invalid username or password"], message: "Authentication failed");

            var token = await GenerateToken(user);

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = user.Id,
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays)
            };

            user.RefreshTokens.Add(refreshToken);
            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
                return Result<TokenModel>.Failure(["can't generate new refresh token"], message: "Generate RefreshToken Successfully");

            var tokenModel = new TokenModel
            {
                Token = token,
                ExpiresOn = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

            return Result<TokenModel>.Success(tokenModel, "Login successful");
        }

        public async Task<Result<TokenModel>> GetRefreshTokenAsync(string? refreshToken)
        {
            if (refreshToken is null)
                return Result<TokenModel>.Failure(["refresh token is required"], message: "Invalid RefreshToken");

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens.Where(r => r.Token == refreshToken))
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken && r.ExpiresOn > DateTime.UtcNow && r.RevokedOn == null));

            if (user is null)
                return Result<TokenModel>.Failure(["refresh token is not exists"], message: "Invalid RefreshToken");

            var oldRefreshToken = user.RefreshTokens.First(r => r.Token == refreshToken);

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays),
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = user.Id
            };

            user.RefreshTokens.Add(newRefreshToken);
            oldRefreshToken.RevokedOn = DateTime.UtcNow;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
                return Result<TokenModel>.Failure(["can't generate new refresh token"], message: "Generate RefreshToken Successfully");

            var tokenModel = new TokenModel
            {
                Token = await GenerateToken(user),
                ExpiresOn = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };

            return Result<TokenModel>.Success(tokenModel, message: "Generate RefreshToken Successfully");
        }

        public async Task<Result> RevokeRefreshTokenAsync(string refreshToken)
        {
            if (refreshToken is null)
                return Result.Failure(["refresh token is required"], message: "Invalid RefreshToken");

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens.Where(r => r.Token == refreshToken))
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken && r.ExpiresOn > DateTime.UtcNow && r.RevokedOn == null));

            if (user is null)
                return Result.Failure(["refresh token is not exists"], message: "Invalid RefreshToken");

            var oldRefreshToken = user.RefreshTokens.First(r => r.Token == refreshToken);

            oldRefreshToken.RevokedOn = DateTime.UtcNow;
            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
                return Result.Failure(["can't generate new refresh token"], message: "Generate RefreshToken Successfully");

            return Result.Success("refresh token revoked Successfully");
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}