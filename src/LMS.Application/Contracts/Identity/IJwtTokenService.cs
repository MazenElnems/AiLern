using LMS.Domain.Entities.Users;
using System.IdentityModel.Tokens.Jwt;

namespace LMS.Application.Contracts.Identity;

public interface IJwtTokenService
{
    Task<(string, DateTime)> GenerateTokenAsync(ApplicationUser user);
}
