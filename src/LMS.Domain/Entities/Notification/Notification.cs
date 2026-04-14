namespace LMS.Domain.Entities.Notification;

public class Notification
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; }
    public NotificationType Type { get; set; }  

    public List<UserNotification> UserNotifications { get; set; } = new();
}
