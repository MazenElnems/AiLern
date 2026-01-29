using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Commands.Assignments.AssignmentUpdateCommands;

public class AssignmentUpdateCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
}
