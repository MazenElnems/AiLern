using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetAttemptsByQuizId;


public class GetAttemptsByQuizIdQuery(Guid quizId) : IRequest<Result<List<GetAttemptsByQuizIdDto>>>
{
    public Guid QuizId { get; } = quizId;
}

