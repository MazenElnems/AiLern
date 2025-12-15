using LMS.Domin.Common;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQuery : BasePagedQuery, IRequest<PaginationResult<GetAllCoursesDto>>
{

}
