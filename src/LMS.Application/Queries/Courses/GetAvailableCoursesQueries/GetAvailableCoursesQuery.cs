using LMS.Domain.Common;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Queries.Courses.GetAvailableCoursesQueries;

public class GetAvailableCoursesQuery : BasePagedQuery, IRequest<PaginationResult<GetAvailableCoursesDto>>
{

}
