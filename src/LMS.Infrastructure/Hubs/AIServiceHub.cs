using LMS.Application.Contracts.ExternalServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Hubs;

public class AIServiceHub : Hub
{
    private readonly ILogger<AIServiceHub> _logger;
    private readonly IAIService _aiService;

    public AIServiceHub(ILogger<AIServiceHub> logger, IAIService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId} , User: {User}", Context.ConnectionId, Context.User);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId} , User: {User}, Exception: {Exception}", Context.ConnectionId, Context.User, exception);
        return base.OnDisconnectedAsync(exception);
    }

    public Task CancelQuestionGenerationJob(string jobId)
    {
        _logger.LogInformation("Canceling question generation job: {JobId} for User: {User}", jobId, Context.User);
        // Implement logic to cancel the job here
        return Task.CompletedTask;
    }
}
