using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Contracts.ExternalServices;
using LMS.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LMS.Infrastructure.ExternalServices.AIService;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<AIService> _logger;

    private readonly Dictionary<string, string> _mineTypes = new Dictionary<string, string>
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    public AIService(IHttpClientFactory factory, ILogger<AIService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = new HttpResponseMessage();
        var result = new AIDeleteProjectResponse();
        try
        {
            using var client = _factory.CreateClient("AIService");

            response = await client.DeleteAsync(
                $"/api/v1/professor/project/{projectId}",
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            result = await response.Content.ReadFromJsonAsync<AIDeleteProjectResponse>(AIServiceJsonOptions.Default)
                       ?? throw new Exception("Failed to deserialize response");

            return result;
        }
        catch (TaskCanceledException ex)    // Timeout
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"DELETE: /api/v1/professor/project/{projectId}"
            );

            throw;
        }
        catch(HttpRequestException ex)  // 4xx & 5xx 
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status Code: {StatusCode}",
                $"DELETE: /api/v1/professor/project/{projectId}",
                response.StatusCode
            );
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"DELETE: /api/v1/professor/project/{projectId}"
            );
            throw;
        }
    }

    public async Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken)
    {
        try
        {
            using var client = _factory.CreateClient("AIService");

            var response = await client.PostAsJsonAsync(
                "/api/v1/data/QA_enhance",
                quizGenerationRequest,
                AIServiceJsonOptions.Default,
                cancellationToken
            );

            var json = JsonSerializer.Serialize(quizGenerationRequest, AIServiceJsonOptions.Default);

            Console.WriteLine(json);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AIQuizGenerationResonse>(AIServiceJsonOptions.Default, cancellationToken)
                   ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", "/api/v1/data/QA_enhance");
            throw;
        }
    }

    public async Task<AIUploadFilesResponse> UploadFileAsync(string projectId, string filename, Stream fileStream, CancellationToken cancellationToken)
    {
        try
        {
            using var client = _factory.CreateClient("AIService");

            var formDataContent = new MultipartFormDataContent();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(_mineTypes[Path.GetExtension(filename)]);

            formDataContent.Add(fileContent, "file", filename);

            var response = await client.PostAsync(
                $"/api/v1/professor/upload_docs/{projectId}",
                formDataContent,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AIUploadFilesResponse>(AIServiceJsonOptions.Default ,cancellationToken) 
               ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", "/api/v1/professor/upload_docs");
            throw;
        }
    }

    public async Task<AIUploadDocsResponse> UploadFilesAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken)
    {
        var result = new AIUploadDocsResponse();
        HttpResponseMessage response = new HttpResponseMessage();

        try
        {
            using var client = _factory.CreateClient("AIService");

            response.Headers.Add("X-WebHook-UpdateFileStatus", "api/webhooks/ai/upload-result");

            response = await client.PostAsJsonAsync(
                "/api/v1/professor/upload_docs_from_presigned_urls",
                uploadDocsRequest,
                AIServiceJsonOptions.Default,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            result = await response.Content.ReadFromJsonAsync<AIUploadDocsResponse>(cancellationToken)
                ?? throw new Exception("Failed to deserialize response");

            return result;
        }
        catch(TimeoutException ex)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                "/api/v1/professor/upload_docs_from_presigned_urls"
            );
            throw;
        }
        catch(HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status: {HttpStatusCode}",
                "/api/v1/professor/upload_docs_from_presigned_urls",
                response.StatusCode
            );
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", "/api/v1/professor/upload_docs_from_presigned_urls");
            throw;
        }
    }
}
