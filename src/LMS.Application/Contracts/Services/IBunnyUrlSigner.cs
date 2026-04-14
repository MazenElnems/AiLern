namespace LMS.Application.Contracts.Services;

public interface IBunnyUrlSigner
{
    string GenerateSignedUrl(
        string baseUrl,
        string tokenKey,
        string filePath,
        TimeSpan validFor
    );
}
