using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentDeleteFileCommands;

public sealed record AssignmentDeleteFileCommand(int AssignmentId, Guid FileId) : IRequest;
