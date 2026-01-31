using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
}
