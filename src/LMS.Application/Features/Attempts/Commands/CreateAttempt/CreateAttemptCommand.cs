using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.CreateAttempt;

public record CreateAttemptCommand(Guid QuizId) : IRequest<Result<AttemptDto>> { }

