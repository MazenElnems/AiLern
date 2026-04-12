using LMS.Domain.Entities.Users;

namespace LMS.Application.Contracts.Identity;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, DateTime expiration);
}
