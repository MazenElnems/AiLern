using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries.GetAttemptsByQuizId;

public record GetAttemptsByQuizIdQuery(
    Guid QuizId) : IRequest<Result<GetAttemptsByQuizIdDto>>
{ }
