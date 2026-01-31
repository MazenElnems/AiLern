using System.Security.Claims;

namespace LMS.Application.Services.Auth.Interfaces;

public interface ITokensService
{
    (string,DateTime) GetAccessTokenAsync(List<Claim> claims);
    string GetRefreshToken();
    //Task<Result> CreateUserAsync(string adminUserName, RegisterDto registerDto);
    //Task<Result<TokenModel>> GetRefreshTokenAsync(string refreshToken);
    //Task<Result> RevokeRefreshTokenAsync(string refreshToken);
}
