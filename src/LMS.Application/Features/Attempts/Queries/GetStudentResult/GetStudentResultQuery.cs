using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries.GetAttempt;

public record GetStudentResultQuery(
    Guid AttemptId
) : IRequest<Result<AttemptResultDto>>
{ }
