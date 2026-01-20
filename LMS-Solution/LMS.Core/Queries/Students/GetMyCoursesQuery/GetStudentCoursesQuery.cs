using LMS.Domain.Common;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Students.GetMyCoursesQuery;

public class GetStudentCoursesQuery : BasePagedQuery, IRequest<List<GetStudentCoursesDto>>
{

}
