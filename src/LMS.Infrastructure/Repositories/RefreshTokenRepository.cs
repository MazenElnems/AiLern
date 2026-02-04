using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
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