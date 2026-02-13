using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Sections;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class SectionCreateCommand : IRequest<Result<SectionDto>>
{
    public string Title { get; set; }
    public int SectionNumber { get; set; }
    public int CourseId { get; set; }
}
