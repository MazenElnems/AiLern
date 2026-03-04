using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public sealed record GetCourseSectionsQuery(int CourseId) : IRequest<Result<List<CourseSectionsDto>>>
{
}
