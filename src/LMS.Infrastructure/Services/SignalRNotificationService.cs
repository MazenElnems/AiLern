using LMS.Application.Contracts.Services;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(string group, string title, string message)
    {
        await _hubContext.Clients.Group(group).SendAsync("recieveNotification", title, message);
    }

    public async Task NotifyAsync(int userId, string title, string message)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("recieveNotification", title, message);
    }
}
