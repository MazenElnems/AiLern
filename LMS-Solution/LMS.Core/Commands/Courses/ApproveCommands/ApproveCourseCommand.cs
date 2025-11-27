using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Commands.Courses.ApproveCommands;

public class ApproveCourseCommand(int id): IRequest
{
    [JsonIgnore]
    public int Id { get; set; } = id;
}
