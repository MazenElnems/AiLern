using LMS.Core.Common;
using LMS.Core.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAvailableCoursesQueries;

public class GetAvailableCoursesQuery : BasePagedQuery, IRequest<List<GetAvailableCoursesDto>>
{

}
