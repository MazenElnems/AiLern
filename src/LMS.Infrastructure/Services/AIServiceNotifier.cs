using LMS.Application.Contracts.Services;
using LMS.Domain.Enums;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services;

internal class AIServiceNotifier : IAIServiceNotifier
{
    private readonly IHubContext<AIServiceHub> _hub;
    private readonly ILogger<AIServiceNotifier> _logger;

    public AIServiceNotifier(IHubContext<AIServiceHub> hub, ILogger<AIServiceNotifier> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task NotifyQuestionGeneratedAsync(int userId, int questionsCount, bool completed, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notifying user {UserId} about generated questions. Count: {QuestionsCount}, Completed: {Completed}", userId, questionsCount, completed);
        await _hub.Clients.User(userId.ToString()).SendAsync("QuestionsGenerated", questionsCount, completed, cancellationToken);
    }

    public async Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notifying user {UserId} about status change for file {FileId} to {Status}", userId, fileId, status);
        await _hub.Clients.User(userId).SendAsync("StatusUpdated", fileId, status.ToString(), cancellationToken);
    }
}
