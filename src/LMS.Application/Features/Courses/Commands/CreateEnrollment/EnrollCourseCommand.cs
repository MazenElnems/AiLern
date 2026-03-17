using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.CreateEnrollment;

public class EnrollCourseCommand(int studentId ,int courseId) : IRequest<Result>
{
    public int StudentId { get; set; } = studentId;
    public int CourseId { get; set; } = courseId;
}
