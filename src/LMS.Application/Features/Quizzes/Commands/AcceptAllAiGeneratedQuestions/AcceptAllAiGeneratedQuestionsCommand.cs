using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.AcceptAllAiGeneratedQuestions;

public record AcceptAllAiGeneratedQuestionsCommand(Guid QuizId) : IRequest<Result>;
