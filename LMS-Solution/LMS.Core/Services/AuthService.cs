using LMS.API.ConfigurationOptions;
using LMS.Core.ConfigurationOptions;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoryContracts;
using LMS.Core.Models;
using LMS.Core.Models.DTOs;
using LMS.Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly JwtOptions _jwt;
        private readonly RefreshTokenSettings _refreshToken;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwt, IOptions<RefreshTokenSettings> refreshTokenOptions, IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _refreshToken = refreshTokenOptions.Value;
            _refreshTokenRepository = refreshTokenRepository;
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
                // Clean up: delete the user if role assignment fails
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

            // TODO: Generate refreshtoken
            var tokenModel = new TokenModel
            {
                Token = token,
                ExpiresOn = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays)
            };

            return Result<TokenModel>.Success(tokenModel, "Login successful");
        }

        public async Task<Result<TokenModel>> GetRefreshTokenAsync(string? refreshToken)
        {
            if (refreshToken is null)
                return Result<TokenModel>.Failure(["refresh token is required"], message: "Invalid RefreshToken");

            var user = await _refreshTokenRepository.GetUserByRefreshToken(refreshToken);

            if (user is null)
                return Result<TokenModel>.Failure(["refresh token is not exists"], message: "Invalid RefreshToken");

            var oldRefreshToken = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshToken);

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshToken.DurationInDays),
                CreatedOn = DateTime.UtcNow,
                ApplicationUserId = user.Id
            };

            int rowsAffected = await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshToken);

            if(rowsAffected < 1)
                return Result<TokenModel>.Failure(["can't generate new refresh token"], message: "Generate RefreshToken Successfully");

            await _refreshTokenRepository.RevokeRefreshTokenAsync(oldRefreshToken);

            var tokenModel = new TokenModel
            {
                Token = await GenerateToken(user),
                ExpiresOn = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };

            return Result<TokenModel>.Success(tokenModel, message: "Generate RefreshToken Successfully");
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
