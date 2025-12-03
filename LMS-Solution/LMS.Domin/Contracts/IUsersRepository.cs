using LMS.Domin.Entities;
using Microsoft.AspNetCore.Identity;

namespace LMS.Domin.Contracts;

public interface IUsersRepository
{
    Task<(List<ApplicationUser>,int)> GetUsersByRoleIdAsync(int roleId, string sortBy, string order, int pageNo = 1, int pageSize = 10);
    Task AddRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string  refreshToken, bool includeUser = false);
    Task<int> CommitAsync();
    Task<Student?> GetStudentByStudentId(int studentId);
}