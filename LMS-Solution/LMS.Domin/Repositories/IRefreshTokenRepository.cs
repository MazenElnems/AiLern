using LMS.Domin.Entities;

namespace LMS.Domin.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenAsyn(string refreshToken);
    Task<RefreshToken?> GetRefreshTokenWithUserAsync(string refreshToken);
}
