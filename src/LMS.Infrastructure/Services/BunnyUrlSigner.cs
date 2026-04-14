using LMS.Application.Contracts.Services;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Infrastructure.Services;

public class BunnyUrlSigner : IBunnyUrlSigner
{
    public string GenerateSignedUrl(string baseUrl, string tokenKey, string filePath, TimeSpan validFor)
    {
        var expires = DateTimeOffset.UtcNow
            .Add(validFor)
            .ToUnixTimeSeconds();

        var path = filePath.StartsWith("/") ? filePath : "/" + filePath;

        var hashInput = $"{tokenKey}{path}{expires}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));

        var token = Convert.ToBase64String(hashBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        return $"{baseUrl}{path}?token={token}&expires={expires}";
    }
}
