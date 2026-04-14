using LMS.Domain.Entities.Users;

namespace LMS.Domain.Entities.Notification;

public class UserNotification
{
    public int UserId { get; set; }
    public Guid NotificationId { get; set; }
    public bool IsRead { get; set; }

    // Navigation Properties
    public ApplicationUser User { get; set; }
    public Notification Notification { get; set; }
}
