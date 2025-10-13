using LMS.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Domain.RepositoryContracts
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string renewToken);
        Task RevokeRefreshTokenAsync(RefreshToken? refreshToken);
        Task<int> AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken);
    }
}
