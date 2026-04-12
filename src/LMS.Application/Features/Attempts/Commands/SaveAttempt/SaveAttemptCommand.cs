using LMS.Application.Common.Results;
using LMS.Application.Features.Attempts.Shared.Requests;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SaveAttempt;

public record SaveAttemptCommand(
    Guid AttemptId,
    List<SaveAttemptAnswerRequest> Answers
) : IRequest<Result>
{ }
