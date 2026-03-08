using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class SectionCreateCommand : IRequest<Result<SectionDto>>
{
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public int CourseId { get; set; }
}
