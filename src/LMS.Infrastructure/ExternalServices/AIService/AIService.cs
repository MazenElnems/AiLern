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

public class AIService(
    IHttpClientFactory factory,
    ILogger<AIService> logger,
    IOptions<AIServiceSettings> aiServiceOptions,
    IOptions<WebhookSettings> webhookSettings
) : IAIService
{
    private readonly IHttpClientFactory _factory = factory;
    private readonly ILogger<AIService> _logger = logger;
    private readonly AIServiceSettings _aiServiceSettings = aiServiceOptions.Value;
    private readonly WebhookSettings _webhookSettings = webhookSettings.Value;

    public async Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken)
    {
        var requestUrl = $"{_aiServiceSettings.DeleteEndpoint}/{projectId}";
        var message = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

        var result = await ExecuteAsync<AIDeleteProjectResponse>(message, cancellationToken);

        return result;
    }

    public async Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken)
    {
        var webhookUrl = BuildWebhookUrl("QuestionsGenerated");

        var message = new HttpRequestMessage(HttpMethod.Post, _aiServiceSettings.QAEndpoint)
        {
            Content = JsonContent.Create(quizGenerationRequest, options: AIServiceJsonOptions.Default)
        };
        message.Headers.Add("X-QuestionGenerated-WebHook", webhookUrl);

        var result = await ExecuteAsync<AIQuizGenerationResonse>(message, cancellationToken);

        return result;
    }

    public async Task<AIUploadDocsResponse> UploadDocsAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken)
    {
        var webhookUrl = BuildWebhookUrl("DocumentsUploaded");

        var message = new HttpRequestMessage(HttpMethod.Post, _aiServiceSettings.UploadEndpoint)
        {
            Content = JsonContent.Create(uploadDocsRequest, options: AIServiceJsonOptions.Default)
        };
        message.Headers.Add("X-WebHook-UpdateFileStatus", webhookUrl);

        var result = await ExecuteAsync<AIUploadDocsResponse>(message, cancellationToken);

        return result;
    }

    private async Task<T> ExecuteAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage();
        try
        {
            using var client = _factory.CreateClient("AIService");

            response = await client.SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result =  await response.Content.ReadFromJsonAsync<T>(cancellationToken) ??
                throw new InvalidOperationException("Failed to deserialize response");

            return result;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "Timeout occurred while requesting {AIServiceEndpoint} endpoint of AI Service.",
                $"{message.RequestUri?.PathAndQuery}"
            );

            throw new AIServiceTimeoutException("The AI service did not respond in time. Please try again later.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP request error occurred while requesting {AIServiceEndpoint} endpoint of AI Service. Status: {HttpStatusCode}",
                $"{message.RequestUri?.PathAndQuery}",
                response.StatusCode
            );

            throw new AIServiceUnAvailableException("The AI service is unavailable. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while requesting {AIServiceEndpoint} endpoint of AI Service.", $"{message.RequestUri?.PathAndQuery}");
            throw;
        }
    }

    private string BuildWebhookUrl(string endpointKey) =>
        $"{_webhookSettings.BaseUrl}/{_webhookSettings.Endpoints[endpointKey]}";
}
