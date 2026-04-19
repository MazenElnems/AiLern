namespace LMS.Application.Contracts.ExternalServices;

public interface IBunnyUrlSigner
{
    string GenerateSignedUrl(
        string baseUrl,
        string tokenKey,
        string path,
        TimeSpan validFor
    );
}
