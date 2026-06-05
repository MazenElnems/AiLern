using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeAttemptByAI;

public record GradeByAICommand(Guid QuizId, List<Guid> AtteptIds) : IRequest<Result>;
