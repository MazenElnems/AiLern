using MediatR;

namespace LMS.Core.Commands.Courses.DeleteCommands;

public class DeleteCourseCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
