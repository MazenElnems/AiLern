using Amazon.Runtime.Internal.Util;
using LMS.Application.Contracts.Services;
using LMS.Domain.Enums;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services;

internal class AIStatusNotifier : IAIStatusNotifier
{
    private readonly IHubContext<AIResourcesHub> _hub;
    private readonly ILogger<AIStatusNotifier> _logger;

    public AIStatusNotifier(IHubContext<AIResourcesHub> hub, ILogger<AIStatusNotifier> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notifying user {UserId} about status change for file {FileId} to {Status}", userId, fileId, status);
        await _hub.Clients.User(userId).SendAsync("StatusUpdated", fileId, status.ToString(), cancellationToken);
    }
}
