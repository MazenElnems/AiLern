using LMS.Core.ConfigurationOptions;
using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories.Courses;
using LMS.Infrastructure.Repositories.Users;
using LMS.Infrastructure.Services.Email;
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
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddTransient<IMailSender,MailSender>();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // DbConext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(cfg =>
        {
            cfg.User.RequireUniqueEmail = true;
            cfg.Password.RequireNonAlphanumeric = true;
            cfg.Password.RequireDigit = true;
            cfg.Password.RequireUppercase = true;
            cfg.Password.RequiredLength = 6;
            cfg.SignIn.RequireConfirmedEmail = true;
        })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
