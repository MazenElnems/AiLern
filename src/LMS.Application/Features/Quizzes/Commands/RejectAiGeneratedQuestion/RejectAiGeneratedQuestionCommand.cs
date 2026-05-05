using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.RejectAiGeneratedQuestion;

public record RejectAiGeneratedQuestionCommand(Guid QuizId, Guid QuestionId) : IRequest<Result>;
