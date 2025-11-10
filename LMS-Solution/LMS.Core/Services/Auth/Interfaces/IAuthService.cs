using LMS.Domin.Entities;
using System.Security.Claims;

namespace LMS.Core.Services.Auth.Interfaces;

public interface IAuthService
{
    (string,DateTime) GetAccessTokenAsync(List<Claim> claims);
    string GetRefreshToken();
    //Task<Result> CreateUserAsync(string adminUserName, RegisterDto registerDto);
    //Task<Result<TokenModel>> GetRefreshTokenAsync(string refreshToken);
    //Task<Result> RevokeRefreshTokenAsync(string refreshToken);
}
