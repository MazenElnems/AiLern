using LMS.Domin.Common;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAvailableCoursesQueries;

public class GetAvailableCoursesQuery : BasePagedQuery, IRequest<PaginationResult<GetAvailableCoursesDto>>
{

}
