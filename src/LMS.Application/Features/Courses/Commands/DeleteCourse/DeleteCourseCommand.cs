using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommand(int id) : IRequest<Result>
{
    public int Id { get; } = id;
}
