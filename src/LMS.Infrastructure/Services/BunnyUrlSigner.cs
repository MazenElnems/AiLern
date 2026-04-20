using LMS.Application.Contracts.ExternalServices;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Infrastructure.Services;

public class BunnyUrlSigner : IBunnyUrlSigner
{
    public string GenerateSignedUrl(string baseUrl, string tokenKey, string filePath, TimeSpan validFor)
    {
        var normalizedPath = filePath.StartsWith('/') ? filePath : "/" + filePath;
        var baseTrimmed = baseUrl.TrimEnd('/');
        var uri = new Uri(baseTrimmed + normalizedPath);

        var expires = DateTimeOffset.UtcNow.Add(validFor).ToUnixTimeSeconds().ToString();
        var signaturePath = uri.AbsolutePath;

        // message = signaturePath + expires
        var message = string.Concat(signaturePath, expires);
        var token = "HS256-" + HmacSha256Base64Url(tokenKey, message);

        var origin = $"{uri.Scheme}://{uri.Authority}";
        return $"{origin}{signaturePath}?token={token}&expires={expires}";
    }

    private static string HmacSha256Base64Url(string key, string message)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hash)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
