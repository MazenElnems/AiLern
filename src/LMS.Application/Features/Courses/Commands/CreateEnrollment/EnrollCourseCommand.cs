using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.CreateEnrollment;

public class EnrollCourseCommand(int courseId) : IRequest<Result>
{
    public int CourseId { get; set; } = courseId;
}
