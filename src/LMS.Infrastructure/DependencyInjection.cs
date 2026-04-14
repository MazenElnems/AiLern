using Amazon.S3;
using Hangfire;
using LMS.Application.Common.Interfaces;
using LMS.Application.Contracts.Identity;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Repositories;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Settings;
using LMS.Domain.Entities.Users;
using LMS.Infrastructure.ExternalServices.AIService;
using LMS.Infrastructure.ExternalServices.AIService.Contracts;
using LMS.Infrastructure.Jobs;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<IEmailSender,EmailSender>();
        services.AddScoped<IBunnyUrlSigner, BunnyUrlSigner>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<IConfirmUploadedFilesJob,ConfirmUploadedFilesJob>();
        services.AddScoped<IRemoveExpiredRefreshTokensJob, RemoveExpiredRefreshTokensJob>();
        services.AddScoped<IBackgroundJobService, HangfireJobService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IQuizPublishSchedulerJob, QuizPublishSchedulerJob>();
        services.AddScoped<IGenerateQuestionsJob, GenerateQuestionsJob>();
        services.AddScoped<IAutoSubmitAttemptJob, AutoSubmitAttemptJob>();
        services.AddScoped<ICalculateStudentScoreJob, CalculateStudentScoreJob>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IAnswersRepository, AnswersRepository>();
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();

        services.Configure<BunnyOptions>(configuration.GetSection("BunnyCDN"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<FrontEndSettings>(configuration.GetSection("FrontEndSettings"));
        services.Configure<AIServiceSettings>(configuration.GetSection("AIServiceSettings"));

        // AutoMapper
        services.AddAutoMapper(cfg => { } ,
            [typeof(DependencyInjection).Assembly]);

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

        // JWT 
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        services.Configure<RefreshTokenOptions>(configuration.GetSection("RefreshTokenOptions"));
        var jwt = configuration.GetSection("JWT").Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt?.Issuer,
                    ValidAudience = jwt?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt?.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

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
