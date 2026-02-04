using AutoMapper;
using LMS.Application.Features.Sections.Commands.CreateSection;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Sections.Mappings;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateCommand, Section>();
    }


}
