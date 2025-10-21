using Microsoft.AspNetCore.Identity;

namespace LMS.Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Role { get; set; }

        // Navigation property
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
