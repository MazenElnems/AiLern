namespace LMS.Domain.Interfaces;

public interface IBunnyUrlSigner
{
    string GenerateSignedUrl(
        string baseUrl,
        string tokenKey,
        string filePath,
        TimeSpan validFor
    );
}
