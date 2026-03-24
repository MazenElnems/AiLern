using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetAttemptsByQuizId;


public class GetAttemptsByQuizIdQuery(Guid quizId) : IRequest<Result<GetAttemptsByQuizIdDto>>
{
    public Guid QuizId { get; } = quizId;
}

