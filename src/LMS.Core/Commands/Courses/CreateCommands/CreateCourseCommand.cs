using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Commands.Courses.CreateCommands;

public class CreateCourseCommand : IRequest<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
