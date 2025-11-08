using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Infrastructure.Data;
using LMS.Shared.Domain.Entities;
using LMS.Shared.DTOs.Users;
using LMS.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories.UsersManagement
{
    public class UserManagementRepository : IUsersRepository
    {
        private readonly AppDbContext _db;
        //private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserManagementRepository(AppDbContext db/*, RoleManager<IdentityRole<int>> roleManager*/)
        {
            _db = db;
            //_roleManager = roleManager;
        }


        public async Task<List<ApplicationUser>> GetUsersByRoleIdAsync(int roleId,string sortBy,string order, int pageNo = 1, int pageSize = 10)
        {
            IQueryable<ApplicationUser> query = _db.Users;

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

            // Check if Role not existing 
            if (role == null)
                return null!;

            // filter
            query = query.Where(u => u.Role == role.Name);

            if(sortBy !=null && order != null)
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
}
