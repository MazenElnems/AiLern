namespace LMS.Application.Contracts.ExternalServices;

public interface IBunnyUrlSigner
{
    string GenerateSignedUrl(
        string path,
        TimeSpan validFor
    );

    string GetUrl(string path);
}
