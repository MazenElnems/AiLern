using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoryContracts;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public RefreshTokenRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _db.RefreshTokens.Add(refreshToken);
            return await _db.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string renewToken)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == renewToken);
        }

        public Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken)
        {
            return _db.Users
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken && r.ExpiresOn > DateTime.UtcNow && r.RevokedOn != null));
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken? refreshToken)
        {
            refreshToken.RevokedOn = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }
}
