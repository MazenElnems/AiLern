using MediatR;

namespace LMS.Core.Commands.Courses.CourseEnrollmentsCommands;

public class EnrollCourseCommand(int courseId) : IRequest
{
    public int CourseId { get; set; } = courseId;
}
