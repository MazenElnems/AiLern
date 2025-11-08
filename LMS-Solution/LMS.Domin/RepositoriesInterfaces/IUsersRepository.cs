using LMS.Domin.Entities;

namespace LMS.Domin.RepositoriesInterfaces;

public interface IUsersRepository
{
    Task<List<ApplicationUser>> GetUsersByRoleIdAsync(int roleId, string sortBy, string order, int pageNo = 1, int pageSize = 10);
}