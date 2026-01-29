using MediatR;

namespace LMS.Application.Commands.Assignments.ConfirmAssignmentUploadCommands;

public class ConfirmAssignmentUploadCommand : IRequest
{
    public int AssignmentId { get; set; }
}
