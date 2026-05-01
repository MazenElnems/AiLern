using LMS.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Infrastructure.Hubs;

public class AIResourcesHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
