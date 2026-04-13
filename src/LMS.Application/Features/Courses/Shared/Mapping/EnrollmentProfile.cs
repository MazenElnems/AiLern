using AutoMapper;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Domain.Entities.Courses;

namespace LMS.Application.Features.Courses.Shared.Mapping;

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        CreateMap<Enrollment, GetEnrolledStudentsDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Student.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Student.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Student.PhoneNumber));
    }
}
