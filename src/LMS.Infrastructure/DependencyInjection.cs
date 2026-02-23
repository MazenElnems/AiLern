using Amazon.S3;
using Hangfire;
using LMS.Application.ConfigurationOptions;
using LMS.Domain.Entities.Users;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Identity;
using LMS.Infrastructure.Jobs;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWasabiService, WasabiService>();
        services.AddScoped<IMailSender,MailSender>();
        services.AddScoped<IBunnyUrlSigner, BunnyUrlSigner>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IBackgroundService, HangfireJobService>();

        services.Configure<BunnyOptions>(configuration.GetSection("BunnyCDN"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        services.AddHangfireServer();

        // DbConext
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString)
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
