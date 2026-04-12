using Amazon.S3;
using Amazon.S3.Model;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services;

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

    public async Task DeleteFileAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting file from Wasabi: {Key}", key);
            var request = new DeleteObjectRequest
            {
                BucketName = _wasabiSettings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }
        catch(AmazonS3Exception ex) 
        {
            _logger.LogError(ex, "Error deleting file from Wasabi: {Key}", key);
            throw;
        }
    }

    public async Task<Dictionary<string, Stream>> GetFileStreamAsync(List<string> keys)
    {
        try
        {
            var tasks = keys.Distinct().Select(async key =>
            {
                var request = new GetObjectRequest
                {
                    BucketName = _wasabiSettings.BucketName,
                    Key = key
                };

                var response = await _s3Client.GetObjectAsync(request);
                return new KeyValuePair<string, Stream>(key, response.ResponseStream);
            });

            var responses = await Task.WhenAll(tasks);

            return responses.ToDictionary(x => x.Key, x => x.Value);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from Wasabi: {Keys}", string.Join(", ", keys));
            throw;
        }
    }

    public async Task UploadFilesAsync(List<Stream> streams, List<string> keys)
    {
        try
        {
            for(int i = 0; i < keys.Count; i++ ) 
            {
                var request = new PutObjectRequest
                {
                    BucketName = _wasabiSettings.BucketName,
                    Key = keys[i],
                    InputStream = streams[i]
                };
                await _s3Client.PutObjectAsync(request);
            }
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error uploading files to Wasabi: {Keys}", string.Join(", ", keys));
            throw;
        }
    }
}
