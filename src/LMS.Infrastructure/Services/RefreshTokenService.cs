using LMS.Domain.Interfaces;
using System.Security.Cryptography;

namespace LMS.Infrastructure.Services;

internal class RefreshTokenService : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
