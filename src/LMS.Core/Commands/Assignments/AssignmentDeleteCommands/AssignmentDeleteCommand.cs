using MediatR;

namespace LMS.Core.Commands.Assignments.AssignmentDeleteCommands;

public sealed record AssignmentDeleteCommand(int Id) : IRequest;
