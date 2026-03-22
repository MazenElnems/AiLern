using LMS.Application.Common.Results.Generic;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.CreateAttempt;

public record CreateAttemptCommand(Guid QuizId) : IRequest<Result<Guid>> { }

