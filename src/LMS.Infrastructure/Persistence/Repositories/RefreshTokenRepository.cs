using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task RemoveExpiredRefreshTokensAsync()
    {
        await _context.RefreshTokens.Where(x => x.ExpiresOn <= DateTime.UtcNow || x.RevokesOn != null).ExecuteDeleteAsync();
    }

    public Task<RefreshToken?> GetRefreshTokenAsyn(string refreshToken)
    {
        return _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresOn > DateTime.UtcNow && rt.RevokesOn == null);
    }

    public Task<RefreshToken?> GetRefreshTokenWithUserAsync(string refreshToken)
    {
        return _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresOn > DateTime.UtcNow && rt.RevokesOn == null);
    }
}