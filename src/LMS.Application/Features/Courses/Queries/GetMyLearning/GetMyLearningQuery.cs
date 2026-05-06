using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetMyLearning;

public record GetMyLearningQuery(int PageNo, int PageSize)
    : IRequest<Result<PaginationResult<GetMyLearningDto>>>;
