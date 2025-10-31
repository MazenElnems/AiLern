using LMS.API.ConfigurationOptions;
using LMS.Core.Domain.Entities;
using LMS.Core.Services.Authentication;
using LMS.Core.Services.Authentication.Interfaces;
using LMS.Core.Services.Courses;
using LMS.Core.Services.Courses.Interfaces;
using LMS.Core.Users;
using LMS.Shared.ConfigurationOptions;
using LMS.Shared.DTOs.Courses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace LMS.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Custom Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IUserContext, UserContext>();

        // JWT 
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshTokenSettings"));
        var jwt = configuration.GetSection("JWT").Get<JwtOptions>();

        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //    .AddJwtBearer(o =>
        //    {
        //        o.RequireHttpsMetadata = true;
        //        o.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = jwt?.Issuer,
        //            ValidAudience = jwt?.Audience,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt?.Key)),
        //            ClockSkew = TimeSpan.Zero
        //        };
        //    });

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<CreateCourseDto, Course>();

            cfg.CreateMap<Course, GetCourseDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.Course, opt => opt.MapFrom(src => src.Section == null ? null : src.Section.Name))
               .ForMember(dto => dto.Owner, opt => opt.MapFrom(src => src.Admin == null ? null : src.Admin.UserName))
               .ForMember(dto => dto.CourseStatus, opt => opt.MapFrom(src => src.CourseStatus.ToString()));

            cfg.CreateMap<Course, GetAllCoursesDto>();
        });

        return services;
    }
}
