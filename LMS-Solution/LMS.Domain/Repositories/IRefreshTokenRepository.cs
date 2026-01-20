using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenAsyn(string refreshToken);
    Task<RefreshToken?> GetRefreshTokenWithUserAsync(string refreshToken);
}
