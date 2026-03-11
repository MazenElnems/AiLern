using LMS.Infrastructure.ExternalServices.AIService.Contracts;
using LMS.Infrastructure.ExternalServices.AIService.Models;
using LMS.Infrastructure.ExternalServices.AIService.Requests;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LMS.Infrastructure.ExternalServices.AIService;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _factory;
    private readonly Dictionary<string, string> _mineTypes = new Dictionary<string, string>
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory factory, ILogger<AIService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest)
    {
        try
        {
            using var client = _factory.CreateClient("AIService");

            client.BaseAddress = new Uri($"http://localhost:5000/api/v1/data/QA_enhance/");

            var response = await client.PostAsJsonAsync(client.BaseAddress, quizGenerationRequest);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AIQuizGenerationResonse>()
                   ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while request QA_enhance endpoint");
            throw;
        }
    }

    public async Task<AIUploadFilesResponse> UploadFileAsync(string projectId, string filename, Stream fileStream)
    {
        try
        {
            using var client = _factory.CreateClient("AIService");

            client.BaseAddress = new Uri($"http://localhost:5000/api/v1/professor/upload_docs/{projectId}");

            var form = new MultipartFormDataContent();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(_mineTypes[Path.GetExtension(filename)]);

            form.Add(fileContent, "file", filename);

            var response = await client.PostAsync(client.BaseAddress, form);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AIUploadFilesResponse>()
                   ?? throw new Exception("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while request Upload_Docs endpoint");
            throw;
        }
    }
}

