using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Sections;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class SectionCreateCommand : IRequest<Result<SectionDto>>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public int SectionNumber { get; set; }
    [Required]
    public int CourseId { get; set; }
}
