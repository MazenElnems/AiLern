using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Application.Commands.Courses.UpdateCommands;

public class UpdateCourseDetailsCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
