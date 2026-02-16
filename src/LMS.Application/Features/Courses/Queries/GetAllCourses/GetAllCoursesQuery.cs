using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQuery : BasePaginatedQuery, IRequest<Result<PaginationResult<GetAllCoursesDto>>>
{

}
