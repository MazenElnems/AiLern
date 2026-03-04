using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.DeleteQuiz;

public sealed record DeleteQuizCommand(Guid Id) : IRequest<Result>;
