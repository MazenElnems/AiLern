using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Sections;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public sealed record GetCourseSectionsQuery(int CourseId) : IRequest<Result<List<CourseSectionsDto>>>
{
}
