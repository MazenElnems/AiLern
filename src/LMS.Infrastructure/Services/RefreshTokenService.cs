using LMS.Application.Contracts.Identity;
using LMS.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace LMS.Infrastructure.Services;

internal class RefreshTokenService(IOptions<RefreshTokenOptions> refreshToken) : IRefreshTokenService
{
    private readonly RefreshTokenOptions _refreshToken = refreshToken.Value;

    public (string, DateTime) GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return (Convert.ToBase64String(randomNumber), DateTime.UtcNow.AddDays(_refreshToken.DurationInDays));
    }
}
