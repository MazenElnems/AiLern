using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetAiGradingDashboard;

public class GetAiGradingDashboardQuery : IRequest<Result<AiGradingDashboardDto>>
{

}
