using LMS.Application.Common.Results;
using LMS.Application.Features.Quizzes.Shared.Requests;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpsertQuestions;

public record UpsertQuestionsCommand (
    Guid QuizId,
    List<QuestionUpsertRequest> Questions   
) : IRequest<Result>
{ }
