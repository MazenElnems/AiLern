using LMS.Domin.Entities;

namespace LMS.Domin.Repositories;

public interface IUsersRepository : IBaseRepository<ApplicationUser> 
{
    Task<RefreshToken?> GetRefreshTokenAsync(string  refreshToken, bool includeUser = false);
    Task<Student?> GetStudentByStudentId(int studentId);
    Task RevokeRefreshTokensByUserIdAsync(int userId);
}