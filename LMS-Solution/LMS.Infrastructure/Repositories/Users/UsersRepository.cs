using LMS.Core.Constants;
using LMS.Domin.Entities;
using LMS.Domin.RepositoriesInterfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories.Users;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _db;

    public UsersRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleIdAsync(int roleId, string sortBy, string order, int pageNo = 1, int pageSize = 10)
    {
        IQueryable<ApplicationUser> query = _db.Users;

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
            return null!;

        query = query.Where(u => u.Role == role.Name);

        if (sortBy != null && order != null)
        {
            query = (sortBy.ToLower(), order.ToLower()) switch
            {
                (UserManagementSortByOptions.FullName, SortOrderOptions.ASC) => query.OrderBy(u => u.FullName),
                (UserManagementSortByOptions.FullName, SortOrderOptions.DESC) => query.OrderByDescending(u => u.FullName),
                (UserManagementSortByOptions.UserName, SortOrderOptions.ASC) => query.OrderBy(u => u.UserName),
                (UserManagementSortByOptions.UserName, SortOrderOptions.DESC) => query.OrderByDescending(u => u.UserName),
                _ => query
            };
        }

        query = query
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }
}