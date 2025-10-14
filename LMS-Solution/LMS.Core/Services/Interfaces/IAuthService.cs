using LMS.Core.Models;
using LMS.Core.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<TokenModel>> GetTokenAsync(LoginDto loginDto);
        Task<Result> CreateUserAsync(string adminUserName, RegisterDto registerDto);
        Task<Result<TokenModel>> GetRefreshTokenAsync(string refreshToken);
        Task<Result> RevokeRefreshTokenAsync(string refreshToken);
    }
}
