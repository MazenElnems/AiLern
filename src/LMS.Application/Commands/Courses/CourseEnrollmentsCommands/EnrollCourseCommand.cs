using MediatR;

namespace LMS.Application.Commands.Courses.CourseEnrollmentsCommands;

public class EnrollCourseCommand(int courseId) : IRequest
{
    public int CourseId { get; set; } = courseId;
}
