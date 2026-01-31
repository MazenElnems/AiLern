using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Students.Queries.GetMyCourses;

public class GetStudentCoursesQuery : BasePagedQuery, IRequest<Result<List<GetStudentCoursesDto>>>
{

}
