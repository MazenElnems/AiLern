using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.DTO;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public sealed record GetSectionQuery(Guid sectionId) : IRequest<Result<CourseSectionsDto>>

{
}
