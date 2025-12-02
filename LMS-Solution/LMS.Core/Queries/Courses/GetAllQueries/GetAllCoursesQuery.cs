using LMS.Core.Common;
using LMS.Core.Constants;
using LMS.Core.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQuery : BasePagedQuery, IRequest<List<GetAllCoursesDto>>
{

}
