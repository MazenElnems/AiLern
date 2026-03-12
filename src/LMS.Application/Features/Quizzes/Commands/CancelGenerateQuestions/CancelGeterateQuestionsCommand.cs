using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CancelGenerateQuestions;

public record CancelGeterateQuestionsCommand(Guid id) : IRequest<Result>;
