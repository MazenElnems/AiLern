using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public record GetSectionQuery(
    Guid sectionId
) : IRequest<Result<CourseSectionsDto>>
{ }
