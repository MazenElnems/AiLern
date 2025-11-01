using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Commands.Courses.CreateCommands;

public class CreateCourseCommand : IRequest<int>
{
    [Required(ErrorMessage = "Course code is required.")]
    [StringLength(7, ErrorMessage = "Course code must not exceed 7 characters.")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Course name is required.")]
    public string Name { get; set; }
    public string? Description { get; set; }
}
