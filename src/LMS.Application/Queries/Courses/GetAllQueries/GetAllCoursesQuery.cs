using LMS.Domain.Common;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Queries.Courses.GetAllQueries;

public class GetAllCoursesQuery : BasePagedQuery, IRequest<PaginationResult<GetAllCoursesDto>>
{

}
