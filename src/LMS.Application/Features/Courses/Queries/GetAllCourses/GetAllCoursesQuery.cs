using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQuery : BasePagedQuery, IRequest<Result<PaginationResult<GetAllCoursesDto>>>
{

}
