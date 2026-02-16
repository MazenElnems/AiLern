using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Students.Queries.GetMyCourses;

public class GetStudentCoursesQuery : BasePaginatedQuery, IRequest<Result<List<GetStudentCoursesDto>>>
{

}
