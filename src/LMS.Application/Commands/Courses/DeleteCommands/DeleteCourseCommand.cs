using MediatR;

namespace LMS.Application.Commands.Courses.DeleteCommands;

public class DeleteCourseCommand(int id) : IRequest
{
    public int Id { get; } = id;
}
