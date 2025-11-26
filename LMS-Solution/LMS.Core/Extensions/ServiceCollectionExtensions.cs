using FluentValidation;
using FluentValidation.AspNetCore;
using LMS.Core.Commands.Admins.CreateAdminCommands;
using LMS.Core.Commands.Instructors.CreateInstructorsCommands;
using LMS.Core.Commands.Students.CreateCommands;
using LMS.Core.ConfigurationOptions;
using LMS.Core.CurrentUser;
using LMS.Core.DTOs.Courses;
using LMS.Core.DTOs.Users;
using LMS.Core.Services.Auth;
using LMS.Core.Services.Auth.Interfaces;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
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
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAuthService, AuthService>();

        //MediatR
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        // Validator
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<CreateStudentCommandValidator>();

        // JWT 
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        services.Configure<RefreshTokenOptions>(configuration.GetSection("RefreshTokenOptions"));
        var jwt = configuration.GetSection("JWT").Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            // Prevent Login without Email Confirmation
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

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<CreateCourseDto, Course>();
            cfg.CreateMap<CreateStudentCommand, Student>();
            cfg.CreateMap<CreateInstructorCommand, Instructor>();

            cfg.CreateMap<CreateAdminCommand, Admin>()
                .ForMember(a => a.AdminLevel, opt => opt.MapFrom(src => Enum.Parse<AdminLevels>(src.AdminLevel)));

            cfg.CreateMap<Course, GetCourseDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.Course, opt => opt.MapFrom(src => src.Section == null ? null : src.Section.Name))
               .ForMember(dto => dto.Owner, opt => opt.MapFrom(src => src.Admin == null ? null : src.Admin.UserName))
               .ForMember(dto => dto.CourseStatus, opt => opt.MapFrom(src => src.CourseStatus.ToString()));

            cfg.CreateMap<Course, GetApprovedCoursesDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.Course, opt => opt.MapFrom(src => src.Section == null ? null : src.Section.Name));

            cfg.CreateMap<ApplicationUser, GetUsersByRoleDto>();
            cfg.CreateMap<Course, GetAllCoursesDto>();
            cfg.CreateMap<ApplicationUser, GetUserByIdDto>();
        });

        return services;
    }
}
