using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.DeleteAssignment;

public sealed record AssignmentDeleteCommand(int Id) : IRequest<Result>;
