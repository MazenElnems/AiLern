namespace LMS.Domain.Models.Notification;

public class NotificationEmailModel
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
}
