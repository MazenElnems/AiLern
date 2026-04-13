using Microsoft.AspNetCore.Identity;

namespace LMS.Domain.Entities.Users;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
