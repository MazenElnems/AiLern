using LMS.Core.CurrentUser;
using LMS.Core.DTOs.Courses;
using LMS.Core.DTOs.Users;
using LMS.Domin.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace LMS.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Custom Services
        services.AddScoped<IUserContext, UserContext>();

        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<CreateCourseDto, Course>();

            cfg.CreateMap<Course, GetCourseDto>()
               .ForMember(dto => dto.InstructorName, opt => opt.MapFrom(src => src.Instructor.UserName))
               .ForMember(dto => dto.Course, opt => opt.MapFrom(src => src.Section == null ? null : src.Section.Name))
               .ForMember(dto => dto.Owner, opt => opt.MapFrom(src => src.Admin == null ? null : src.Admin.UserName))
               .ForMember(dto => dto.CourseStatus, opt => opt.MapFrom(src => src.CourseStatus.ToString()));

            cfg.CreateMap<ApplicationUser, GetUsersByRoleDto>();
            cfg.CreateMap<Course, GetAllCoursesDto>();
            cfg.CreateMap<ApplicationUser, GetUserByIdDto>();
        });

        return services;
    }
}
