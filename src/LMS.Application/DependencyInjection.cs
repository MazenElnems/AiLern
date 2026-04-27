using FluentValidation;
using LMS.Application.Common.Behaviours;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Application.Features.Auth.Commands.Register;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace LMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Custom Services
        services.AddScoped<IUserContext, UserContext>();

        //MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // Pipline Behavior
            cfg.AddOpenBehavior(typeof(BlockAccessDuringQuizBehaviour<,>));   // 1
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));                // 2
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));             // 3
        });

        services.Configure<ApplicationDomain>(configuration.GetSection("Domain"));

        // Validator
        services
            .AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<CreateCourseCommand, Course>();
            cfg.CreateMap<Course, GetCourseDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.ImageUrl, opt => opt.MapFrom(src => src.ImageStoragePath == null ? null : $"https://ailern-content.b-cdn.net/{src.ImageStoragePath}"));

            cfg.CreateMap<Course, GetStudentCoursesDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.ImageUrl, opt => opt.MapFrom(src => src.ImageStoragePath == null ? null : $"https://ailern-content.b-cdn.net/{src.ImageStoragePath}"));

            cfg.CreateMap<Course, GetInstructorCoursesDto>()
               .ForMember(dto => dto.ImageUrl, opt => opt.MapFrom(src => src.ImageStoragePath == null ? null : $"https://ailern-content.b-cdn.net/{src.ImageStoragePath}"));

            cfg.CreateMap<Course, GetAvailableCoursesDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            cfg.CreateMap<Student, GetEnrolledStudentsDto>();

            cfg.CreateMap<ApplicationUser, GetUsersByRoleDto>();
            cfg.CreateMap<Course, GetAllCoursesDto>();
            cfg.CreateMap<ApplicationUser, GetUserByIdDto>();

            cfg.CreateMap<Assignment, AssignmentWithFilesDto>();

            cfg.CreateMap<AssignmentSubmission, AssignmentSubmissionDto>();
            cfg.CreateMap<Assignment, AssignmentDto>();
            cfg.CreateMap<AssignmentSubmission, AssignmentSubmissionDto>();
        }, [typeof(DependencyInjection).Assembly]);

        return services;
    }
}
