using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.Requests;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetPendingAiGeneratedQuestions;

public record GetPendingAiGeneratedQuestionsQuery(Guid QuizId) : IRequest<Result<List<QuestionDto>>>;
