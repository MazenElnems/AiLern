using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.Requests;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public record CreateQuizCommand(
    int CourseId,
    QuizRequest Quiz
): IRequest<Result<Guid>>
{ }

