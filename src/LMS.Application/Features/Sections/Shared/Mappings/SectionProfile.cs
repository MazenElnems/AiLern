using AutoMapper;
using LMS.Application.Features.Sections.Commands.CreateSection;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Domain.Entities.Courses;

namespace LMS.Application.Features.Sections.Shared.Mappings;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateCommand, Section>();

        CreateMap<Section, SectionDto>();
        CreateMap<Section, CourseSectionsDto>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));
    }
}
