using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.AcceptAiGeneratedQuestion;

public record AcceptAiGeneratedQuestionCommand(Guid QuizId, Guid QuestionId) : IRequest<Result>;
