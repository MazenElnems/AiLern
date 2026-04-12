using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.Requests;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public record UpdateQuizCommand(
    Guid QuizId,
    QuizRequest Quiz
) : IRequest<Result<Guid>>
{ }
