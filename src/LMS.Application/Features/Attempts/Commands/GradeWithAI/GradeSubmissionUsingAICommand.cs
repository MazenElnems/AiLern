using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeWithAI;

public record GradeSubmissionUsingAICommand(Guid QuizId, List<Guid> AttemptIds) : IRequest<Result>;
