using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Dashboards.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Dashboards.Queries.GetQuizDashboard;

public sealed record GetQuizDashboardQuery(Guid quizId) : IRequest<Result<QuizDashboardDto>>;

