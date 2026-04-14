namespace LMS.Application.Contracts.Services;

public interface INotificationService
{
    Task NotifyAsync(string group, string title, string message);
    Task NotifyAsync(int userId,  string title, string message);
}
