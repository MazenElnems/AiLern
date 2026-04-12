using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetQuiz;

public record GetQuizByIdQuery(
    Guid QuizId
) : IRequest<Result<GetQuizDto>>
{ }
