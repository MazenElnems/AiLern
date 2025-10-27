using LMS.Shared.Models;
using LMS.Shared.DTOs.Authentication;

namespace LMS.Core.Services.Authentication.Interfaces;

public interface IAuthService
{
    Task<Result<TokenModel>> GetTokenAsync(LoginDto loginDto);
    Task<Result> CreateUserAsync(string adminUserName, RegisterDto registerDto);
    Task<Result<TokenModel>> GetRefreshTokenAsync(string refreshToken);
    Task<Result> RevokeRefreshTokenAsync(string refreshToken);
}
