using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Core.Commands.Courses.UpdateCommands;

public class UpdateCourseDetailsCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    [Required]
    public string Code { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
}
