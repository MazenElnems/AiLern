using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.Shared.DTO;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public record GetCourseSectionsQuery(
    int CourseId
) : ICourseContentRequest<Result<List<CourseSectionsDto>>>
{ }
