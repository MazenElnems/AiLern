using LMS.Core.Common;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQuery : BasePagedQuery, IRequest<List<GetAllCoursesDto>>
{

}
