using LMS.Domin.Entities;

namespace LMS.Domin.Repositories;

public interface IUsersRepository
{
    Task<(List<ApplicationUser>,int)> GetUsersByRoleIdAsync(int roleId, string sortBy, string order, int pageNo = 1, int pageSize = 10);
    Task AddRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string  refreshToken, bool includeUser = false);
    Task<int> CommitAsync();
    Task<Student?> GetStudentByStudentId(int studentId);
    Task RevokeRefreshTokensByUserIdAsync(int userId);
}