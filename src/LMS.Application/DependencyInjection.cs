using FluentValidation;
using LMS.Application.Common.Behaviors;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Admins.Commands.CreateAdmin;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Application.Features.Instructors.Commands.CreateInstructor;
using LMS.Application.Features.Students.Commands.CreateStudent;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Application.Features.Users.Shared.DTO;
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
            cfg.AddOpenBehavior(typeof(ExceptionBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.Configure<ApplicationDomain>(configuration.GetSection("Domain"));

        // Validator
        services
            .AddValidatorsFromAssemblyContaining<CreateStudentCommandValidator>();


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

            cfg.CreateMap<AssignmentSubmission, AssignmentSubmissionDto>();
            cfg.CreateMap<Assignment, AssignmentDto>();
            cfg.CreateMap<AssignmentSubmission, AssignmentSubmissionDto>();
        }, [typeof(DependencyInjection).Assembly]);

        return services;
    }
}
