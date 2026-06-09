using LMS.Application.Common.Results;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuestionGradingCriteria;

public record UpdateQuestionGradingConfigCommand(
    Guid QuizId,
    Guid QuestionId,
    QuestionGradingConfigDto GradingConfigDto
): IRequest<Result>
{
}
