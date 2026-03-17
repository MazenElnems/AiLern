using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Instructors.Queries.GetMyCourses;

public class GetInstructorCoursesQuery : IRequest<Result<List<GetInstructorCoursesDto>>>
{
}
