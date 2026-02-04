using FluentValidation;
using LMS.Application.Common.Behaviors;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Admins.Commands.CreateAdmin;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Instructors.Commands.CreateInstructor;
using LMS.Application.Features.Students.Commands.CreateStudent;
using LMS.Application.Services.Auth;
using LMS.Application.Services.Auth.Interfaces;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.DTOs.Sections;
using LMS.Domain.DTOs.Students;
using LMS.Domain.DTOs.Submission;
using LMS.Domain.DTOs.Users;
using LMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;


namespace LMS.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Custom Services
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ITokensService, TokensService>();

        //MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.Configure<ApplicationDomain>(configuration.GetSection("Domain"));

        services.Configure<BunnyOptions>(configuration.GetSection("BunnyCDN"));

        // Validator
        services
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
            cfg.CreateMap<CreateCourseCommand, Course>();
            cfg.CreateMap<CreateStudentCommand, Student>();
            cfg.CreateMap<CreateInstructorCommand, Instructor>();

            cfg.CreateMap<CreateAdminCommand, Admin>();

            cfg.CreateMap<Course, GetCourseDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.CourseStatus, opt => opt.MapFrom(src => src.CourseStatus.ToString()));

            cfg.CreateMap<Course, GetStudentCoursesDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName));

            cfg.CreateMap<Course, GetAvailableCoursesDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            cfg.CreateMap<Student, GetStudentsByCourseIdDto>();

            cfg.CreateMap<ApplicationUser, GetUsersByRoleDto>();
            cfg.CreateMap<Course, GetAllCoursesDto>();
            cfg.CreateMap<ApplicationUser, GetUserByIdDto>();

            cfg.CreateMap<Assignment, AssignmentWithFilesDto>();

            cfg.CreateMap<Assignment, AssignmentDto>();
            cfg.CreateMap<AssignmentSubmission, SubmissionDto>();
            cfg.CreateMap<Section, SectionDto>();
            cfg.CreateMap<Section,CourseSectionsDto>()
            .ForMember(dest =>dest.CourseName,opt =>opt.MapFrom(src =>src.Course.Name));
        }, [typeof(ServiceCollectionExtensions).Assembly]);

        return services;
    }
}
