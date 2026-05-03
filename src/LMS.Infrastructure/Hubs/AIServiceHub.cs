using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Hubs;

public class AIServiceHub : Hub
{
    private readonly ILogger<AIServiceHub> _logger;

    public AIServiceHub(ILogger<AIServiceHub> logger)
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
