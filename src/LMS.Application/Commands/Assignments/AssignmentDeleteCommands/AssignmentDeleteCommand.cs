using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentDeleteCommands;

public sealed record AssignmentDeleteCommand(int Id) : IRequest;
