using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuizStatus;

public record UpdateQuizStatusCommand(
    Guid QuizId,
    QuizStatus Status
) : IRequest<Result>
{

}
