using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Hubs;

public class AIResourcesHub : Hub
{
    private readonly ILogger<AIResourcesHub> _logger;

    public AIResourcesHub(ILogger<AIResourcesHub> logger)
    {
        _logger = logger;
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
}
