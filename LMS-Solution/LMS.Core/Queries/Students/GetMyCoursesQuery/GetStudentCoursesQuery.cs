using LMS.Domin.Common;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Students.GetMyCoursesQuery;

public class GetStudentCoursesQuery : BasePagedQuery, IRequest<List<GetStudentCoursesDto>>
{

}
