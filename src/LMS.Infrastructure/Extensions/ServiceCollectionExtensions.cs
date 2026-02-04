using Amazon.S3;
using LMS.Application.ConfigurationOptions;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Services.BunnyCDN;
using LMS.Infrastructure.Services.Email;
using LMS.Infrastructure.Services.Storage;
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWasabiService, WasabiService>();
        services.AddTransient<IMailSender,MailSender>();
        services.AddTransient<IBunnyUrlSigner, BunnyUrlSigner>();

        services.Configure<BunnyOptions>(configuration.GetSection("BunnyCDN"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // DbConext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .EnableSensitiveDataLogging();
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

        // wasabi storage
        services.Configure<WasabiSettings>(configuration.GetSection("Wasabi"));
        var wasabi = configuration.GetSection("Wasabi").Get<WasabiSettings>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = wasabi.ServiceURL,
                ForcePathStyle = true
            };

            return new AmazonS3Client(
                wasabi.AccessKey,
                wasabi.SecretKey,
                config
            );
        });

        return services;
    }
}
