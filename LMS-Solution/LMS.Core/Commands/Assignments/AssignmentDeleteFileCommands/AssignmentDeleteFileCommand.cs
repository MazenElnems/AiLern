using MediatR;

namespace LMS.Core.Commands.Assignments.AssignmentDeleteFileCommands;

public sealed record AssignmentDeleteFileCommand(int AssignmentId, Guid FileId) : IRequest;
