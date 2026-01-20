using Microsoft.AspNetCore.Identity;

namespace LMS.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; }
    public string Role { get; set; }
}
