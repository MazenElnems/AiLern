namespace LMS.Application.Contracts.ExternalServices;
public interface IWasabiService
{
    Task<string> GeneratePresignedUploadUrlAsync(string key, string contentType, int expirationMinutes = 15, bool secret = true);
    Task<string> GeneratePresignedDownloadUrlAsync(string key, int expirationMinutes = 15, bool secret = true);
    Task<bool> FileExists(string key, bool secret = true);
    Task DeleteFileAsync(string key, CancellationToken cancellationToken, bool secret = true);
    Task<Dictionary<string, Stream>> GetFileStreamAsync(List<string> keys);
    Task UploadFilesAsync(List<Stream> streams, List<string> keys);
}