using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAvailableCourses;

public class GetAvailableCoursesQuery : BasePaginatedQuery, IRequest<Result<PaginationResult<GetAvailableCoursesDto>>>
{

}
