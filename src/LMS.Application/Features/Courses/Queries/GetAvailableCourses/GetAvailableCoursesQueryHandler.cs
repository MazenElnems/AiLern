using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Repositories;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Application.Features.Courses.Shared.DTO;

namespace LMS.Application.Features.Courses.Queries.GetAvailableCourses;

public class GetAvailableCoursesQueryHandler : IRequestHandler<GetAvailableCoursesQuery, Result<PaginationResult<GetAvailableCoursesDto>>>
{
    private readonly ILogger<GetAvailableCoursesQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAvailableCoursesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAvailableCoursesQueryHandler> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<GetAvailableCoursesDto>>> Handle(GetAvailableCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
            {
                return Result<PaginationResult<GetAvailableCoursesDto>>.Failure(DomainErrors.Pagination.InvalidParameters);
            }

            Expression<Func<Course, bool>> predicate = c => true;

            var sortBy = request.SortBy?.ToLower();
            var order = request.Order?.ToLower();
            var isDescending = order != SortOrderOptions.ASC;

            Expression<Func<Course, object>> orderBy = sortBy switch
            {
                CourseSortByOptions.Name => c => c.Name,
                CourseSortByOptions.CreatedAt => c => c.CreatedAt,
                _ => c => c.CreatedAt
            };

            var totalResult = await _unitOfWork.Courses.CountAsync(predicate);

            if (totalResult == 0)
            {
                var emptyResult = new PaginationResult<GetAvailableCoursesDto>(request.PageNumber, request.PageSize, 0, new List<GetAvailableCoursesDto>());
                return Result<PaginationResult<GetAvailableCoursesDto>>.Success(emptyResult);
            }

            var courses = await _unitOfWork.Courses.FilterAsync(
                predicate,
                orderBy,
                isDescending,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                new[] { "Instructor" });

            var dto = _mapper.Map<List<GetAvailableCoursesDto>>(courses);

            return Result<PaginationResult<GetAvailableCoursesDto>>.Success(
                new PaginationResult<GetAvailableCoursesDto>(request.PageNumber, request.PageSize, totalResult, dto));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling GetAvailableCoursesQuery");
            throw;
        }
    }
}
