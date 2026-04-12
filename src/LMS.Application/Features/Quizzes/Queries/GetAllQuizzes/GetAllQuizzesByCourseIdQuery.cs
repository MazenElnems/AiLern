using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes;

public record GetAllQuizzesByCourseIdQuery(
    int CourseId,
    int PageNo,
    int PageSize
) : IRequest<Result<PaginationResult<GetAllQuizDto>>> 
{ }
