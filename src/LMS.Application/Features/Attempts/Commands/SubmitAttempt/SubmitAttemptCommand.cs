using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SubmitAttempt;

public record SubmitAttemptCommand(
    Guid AttemptId
) : IRequest<Result>
{ }