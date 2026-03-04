using AutoMapper;
using LMS.Application.Features.Sections.Commands.CreateSection;
using LMS.Domain.Entities.Courses;

namespace LMS.Application.Features.Sections.Shared.Mappings;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateCommand, Section>();
    }
}
