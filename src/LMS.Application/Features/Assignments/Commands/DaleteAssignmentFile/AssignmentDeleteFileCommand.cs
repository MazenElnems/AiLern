using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.DaleteAssignmentFile;

public sealed record AssignmentDeleteFileCommand(int AssignmentId, Guid FileId) : IRequest<Result>;
