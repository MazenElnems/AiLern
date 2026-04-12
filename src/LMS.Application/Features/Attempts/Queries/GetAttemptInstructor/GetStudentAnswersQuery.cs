using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries.GetAttemptInstructor;

public record GetStudentAnswersQuery(
    Guid AttemptId
) : IRequest<Result<AttemptResultDto>>
{

}
