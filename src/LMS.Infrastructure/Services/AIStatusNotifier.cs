using LMS.Application.Contracts.Services;
using LMS.Domain.Enums;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Infrastructure.Services;

internal class AIStatusNotifier : IAIStatusNotifier
{
    private readonly IHubContext<AIResourcesHub> _hub;

    public AIStatusNotifier(IHubContext<AIResourcesHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyStatusChangeAsync(Guid fileId, string userId, AIStatus status, CancellationToken cancellationToken)
    {
        await _hub.Clients.User(userId).SendAsync("StatusUpdated", fileId, status, cancellationToken);
    }
}
