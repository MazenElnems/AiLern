using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;

public class GetSubmissionsByQuizIdQuery(Guid quizId) : IRequest<Result<List<GetSubmissionsByQuizIdDto>>>
{
    public Guid QuizId { get; } = quizId;
}
