using LMS.Shared.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Domain.RepositoriesInterfaces
{
    public interface IUsersRepository
    {
        Task<List<ApplicationUser>> GetUsersByRoleIdAsync(int roleId,string sortBy,string order, int pageNo = 1, int pageSize = 10);

        //Task DeleteUserRoleAsync(ApplicationUser user,string roleName);
    }
}
