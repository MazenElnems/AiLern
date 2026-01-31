using LMS.Application.Common.Results;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
