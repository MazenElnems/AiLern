using LMS.API.ConfigurationOptions;
using LMS.Core.ConfigurationOptions;
using LMS.Core.Services;
using LMS.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LMS.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Custom Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICookieService, CookieService>();

            // JWT 
            services.Configure<JwtOptions>(configuration.GetSection("JWT"));
            services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshTokenSettings"));
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

            return services;
        }

    }
}
