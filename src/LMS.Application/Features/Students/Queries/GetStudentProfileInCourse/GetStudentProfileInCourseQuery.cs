using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Students.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Students.Queries.GetStudentProfileInCourse;

public class GetStudentProfileInCourseQuery : IRequest<Result<GetStudentProfileInCourseDto>>
{
    public int CourseId { get; set; }
    public int StudentId { get; set; }
}
