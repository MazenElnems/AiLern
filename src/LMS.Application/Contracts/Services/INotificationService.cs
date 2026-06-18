using LMS.Domain.Entities.Notification;

namespace LMS.Application.Contracts.Services;

public interface INotificationService
{
    Task NotifyAsync(int courseId, string title, string message, NotificationType type, string url, string? actionText);
    Task NotifyQuestionRepliedAsync(int userId, int courseId,  string title, string message);
    Task NotifyUserWithEmailAsync(int userId, string title, string message, NotificationType type, string url, string? actionText);
}
