using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries.GetStudentQuestionsAndAswers;

public record GetAttemptQuestionsWithAnswersQuery(
    Guid AttemptId
) : IRequest<Result<AttemptResultForStudentDto>>
{ }
