using Microsoft.EntityFrameworkCore;
using LMS.Domain.Entities.Users;
using LMS.Application.Contracts.Repositories;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class UsersRepository : BaseRepository<ApplicationUser>, IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, bool includeUser = false)
    {
        IQueryable<RefreshToken> query = _context.RefreshTokens;

        if (includeUser)
            query = query
            .Include(r => r.User);

        var result = await query.FirstOrDefaultAsync(r => r.Token == refreshToken && r.ExpiresOn > DateTime.UtcNow && r.RevokesOn == null);
        return result;
    }

    public async Task RevokeRefreshTokensByUserIdAsync(int userId)
    {
        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokesOn == null && rt.ExpiresOn > DateTime.UtcNow)
            .ExecuteUpdateAsync(setter => setter.SetProperty(r => r.RevokesOn, DateTime.UtcNow));
    }
}