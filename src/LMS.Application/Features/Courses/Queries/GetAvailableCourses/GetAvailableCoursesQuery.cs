using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAvailableCourses;

public class GetAvailableCoursesQuery : BasePagedQuery, IRequest<Result<PaginationResult<GetAvailableCoursesDto>>>
{

}
