using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories.Courses;
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

        // DbContext
        services.AddScoped<ICourseRepository, CourseRepository>();

        // DbConext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
        })
        .AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddDefaultTokenProviders();

        services.AddScoped<RoleManager<IdentityRole<int>>>();

        return services;
    }
}
