using LMS.Domain.Entities.Notification;

namespace LMS.Application.Features.Notifications.Shared.DTO
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Url { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
    }
}
