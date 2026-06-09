using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetGradingCriteria;

public record GetAIGradingCriteriaQuery(Guid QuizId) : IRequest<Result<List<AIQuizGradingCriteriaDto>>>;
