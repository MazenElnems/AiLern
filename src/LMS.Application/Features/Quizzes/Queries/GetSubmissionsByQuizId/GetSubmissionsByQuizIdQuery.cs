using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;

public record GetSubmissionsByQuizIdQuery(Guid QuizId, int PageNo, int PageSize, AttemptStatus Status)
    : IRequest<Result<PaginationResult<GetSubmissionsByQuizIdDto>>>
{ }
