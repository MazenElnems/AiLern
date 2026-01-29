using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentPublishCommands;

public sealed record AssignmentPublishCommand(int AssignmentId) : IRequest;
