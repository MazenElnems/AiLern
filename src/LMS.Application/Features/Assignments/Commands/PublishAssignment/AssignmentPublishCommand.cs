using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.PublishAssignment;

public sealed record AssignmentPublishCommand(int AssignmentId) : IRequest<Result>;
