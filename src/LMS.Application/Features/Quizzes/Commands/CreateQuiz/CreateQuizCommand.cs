using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommand : QuizRequest, IRequest<Result<Guid>>
{

}

