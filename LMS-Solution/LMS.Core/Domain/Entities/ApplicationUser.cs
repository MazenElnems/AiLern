using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        // Navigation property
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
