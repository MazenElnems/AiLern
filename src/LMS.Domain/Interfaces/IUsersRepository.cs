using LMS.Domain.Entities.Users;

namespace LMS.Domain.Repositories;

public interface IUsersRepository : IBaseRepository<ApplicationUser> 
{
    Task<RefreshToken?> GetRefreshTokenAsync(string  refreshToken, bool includeUser = false);
    Task RevokeRefreshTokensByUserIdAsync(int userId);
}