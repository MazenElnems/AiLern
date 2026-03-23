using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries;

public record GetAttemptQuestionsQuery(Guid AttemptId) : IRequest<Result<List<AttemptQuestionDto>>>
{ }
