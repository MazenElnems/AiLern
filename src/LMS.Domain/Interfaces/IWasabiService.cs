namespace LMS.Domain.Repositories;
public interface IWasabiService
{
    Task<string> GeneratePresignedUploadUrlAsync(string key, string contentType, int expirationMinutes = 15);
    Task<bool> FileExists(string key);
    Task DeleteFileAsync(string key);
    Task<List<Stream>> GetFileStreamAsync(List<string> keys);
}