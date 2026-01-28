using MediatR;

namespace LMS.Core.Commands.Assignments.AssignmentPublishCommands;

public sealed record AssignmentPublishCommand(int AssignmentId) : IRequest;
