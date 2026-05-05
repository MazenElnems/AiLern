using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Contracts.ExternalServices;
using LMS.Domain.Exceptions;
using LMS.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace LMS.Infrastructure.ExternalServices.AIService;

public class AIService(IHttpClientFactory factory, ILogger<AIService> logger, IOptions<AIServiceSettings> aiServiceOptions, IOptions<WebhookSettings> webhookSettings)
    : IAIService
{
    private readonly IHttpClientFactory _factory = factory;
    private readonly ILogger<AIService> _logger = logger;
    private readonly AIServiceSettings _aiServiceSettings = aiServiceOptions.Value;
    private readonly WebhookSettings _webhookSettings = webhookSettings.Value;

    public async Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = new HttpResponseMessage();
        var result = new AIDeleteProjectResponse();
        try
        {
            using var client = _factory.CreateClient("AIService");

            response = await client.DeleteAsync(
                $"{_aiServiceSettings.DeleteEndpoint}/{projectId}",
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            result = await response.Content.ReadFromJsonAsync<AIDeleteProjectResponse>(AIServiceJsonOptions.Default)
                       ?? throw new Exception("Failed to deserialize response");

            return result;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"DELETE: {_aiServiceSettings.DeleteEndpoint}/{projectId}"
            );

            throw new AIServiceTimeoutException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status Code: {StatusCode}",
                $"DELETE: {_aiServiceSettings.DeleteEndpoint}/{projectId}",
                response.StatusCode
            );

            throw new AIServiceUnAvailableException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"DELETE: {_aiServiceSettings.DeleteEndpoint}/{projectId}"
            );
            throw;
        }
    }

    public async Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken)
    {
        try
        {
            using var client = _factory.CreateClient("AIService");

            var webhookUrl = $"{_webhookSettings.BaseUrl}/{_webhookSettings.Endpoints["QuestionsGenerated"]}";

            client.DefaultRequestHeaders.Add(
                "X-QuestionGenerated-WebHook",
                webhookUrl
            );

            var response = await client.PostAsJsonAsync(
                $"{_aiServiceSettings.QAEndpoint}",
                quizGenerationRequest,
                AIServiceJsonOptions.Default,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AIQuizGenerationResonse>(AIServiceJsonOptions.Default, cancellationToken)
                   ?? throw new Exception("Failed to deserialize response");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"{_aiServiceSettings.QAEndpoint}"
            );

            throw new AIServiceTimeoutException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status Code: {StatusCode}",
                $"{_aiServiceSettings.QAEndpoint}",
                ex.StatusCode
            );

            throw new AIServiceUnAvailableException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", $"{_aiServiceSettings.QAEndpoint}");
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

            var webhookUrl = $"{_webhookSettings.BaseUrl}/{_webhookSettings.Endpoints["DocumentsUploaded"]}";

            client.DefaultRequestHeaders.Add(
                "X-WebHook-UpdateFileStatus",
                webhookUrl
            );

            response = await client.PostAsJsonAsync(
                $"{_aiServiceSettings.UploadEndpoint}",
                uploadDocsRequest,
                AIServiceJsonOptions.Default,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            result = await response.Content.ReadFromJsonAsync<AIUploadDocsResponse>(cancellationToken)
                ?? throw new Exception("Failed to deserialize response");

            return result;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"{_aiServiceSettings.UploadEndpoint}"
            );

            throw new AIServiceTimeoutException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status: {HttpStatusCode}",
                $"{_aiServiceSettings.UploadEndpoint}",
                response.StatusCode
            );

            throw new AIServiceUnAvailableException("The AI service is unavailable. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", $"{_aiServiceSettings.UploadEndpoint}");
            throw;
        }
    }
}
