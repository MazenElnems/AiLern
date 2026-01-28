using MediatR;

namespace LMS.Core.Commands.Assignments.ConfirmAssignmentUploadCommands;

public class ConfirmAssignmentUploadCommand : IRequest
{
    public int AssignmentId { get; set; }
}
