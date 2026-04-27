using LMS.Domain.Entities.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LMS.Domain.Entities.Users;

public class ApplicationUser : IdentityUser<int>
{
    public string? ImageStoragePath { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public List<UserNotification> Notifications { get; set; } = new();
}
