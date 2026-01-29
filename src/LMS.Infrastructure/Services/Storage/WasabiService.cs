using Amazon.S3;
using Amazon.S3.Model;
using LMS.Application.ConfigurationOptions;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services.Storage;

public class WasabiService : IWasabiService
{
    private readonly IAmazonS3 _s3Client;
    private readonly WasabiSettings _wasabiSettings;
    private readonly ILogger<WasabiService> _logger;

    public WasabiService(IAmazonS3 s3Client, IOptions<WasabiSettings> wasabiSettings, ILogger<WasabiService> logger)
    {
        _s3Client = s3Client;
        _wasabiSettings = wasabiSettings.Value;
        _logger = logger;
    }

    public async Task<bool> FileExists(string key)
    {
        try
        {
            _logger.LogInformation("Checking if file exists in Wasabi: {Key}", key);
            await _s3Client.GetObjectMetadataAsync(_wasabiSettings.BucketName, key);
            _logger.LogInformation("File exists in Wasabi: {Key}", key);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(string key, string contentType, int expirationMinutes = 15)
    {
        _logger.LogInformation("Generating pre-signed URL for upload to Wasabi: {Key}", key);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _wasabiSettings.BucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            ContentType = contentType
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task DeleteFileAsync(string key)
    {
        try
        {
            _logger.LogInformation("Deleting file from Wasabi: {Key}", key);
            var request = new DeleteObjectRequest
            {
                BucketName = _wasabiSettings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }
        catch(AmazonS3Exception ex) 
        {
            _logger.LogError(ex, "Error deleting file from Wasabi: {Key}", key);
            throw;
        }
    }
}
