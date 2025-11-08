using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories.Courses;
using LMS.Infrastructure.Repositories.UsersManagement;
using LMS.Infrastructure.Seeders;
using LMS.Infrastructure.Seeders.Interfaces;
using LMS.Shared.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseDataSeeder,CourseDataSeeder>();
        services.AddScoped<IUsersRepository, UserManagementRepository>();

        // DbContext
        services.AddScoped<ICourseRepository, CourseRepository>();

        // DbConext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });


        // Identity
        services.AddIdentityApiEndpoints<ApplicationUser>(cfg =>
        {
            cfg.User.RequireUniqueEmail = true;
            cfg.Password.RequireNonAlphanumeric = true;
            cfg.Password.RequireDigit = true;
            cfg.Password.RequireUppercase = true;
            cfg.Password.RequiredLength = 6;
        }).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}
