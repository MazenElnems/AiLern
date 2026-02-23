using LMS.Domain.Entities.Users;

namespace LMS.Domain.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, DateTime expiration);
}
