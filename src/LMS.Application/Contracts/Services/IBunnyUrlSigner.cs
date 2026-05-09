namespace LMS.Application.Contracts.Services;

public interface IBunnyUrlSigner
{
    string GenerateSignedUrl(
        string path,
        TimeSpan validFor
    );

    string GetUrl(string path);
}
