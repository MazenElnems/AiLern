using LMS.Infrastructure.ExternalServices.AIService.Contracts;
using LMS.Infrastructure.ExternalServices.AIService.Requests;
using LMS.Infrastructure.ExternalServices.AIService.Responses;
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
}

