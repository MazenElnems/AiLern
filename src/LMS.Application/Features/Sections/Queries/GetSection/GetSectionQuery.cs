using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Sections;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public sealed record GetSectionQuery(Guid sectionId) : IRequest<Result<CourseSectionsDto>>

{
}
