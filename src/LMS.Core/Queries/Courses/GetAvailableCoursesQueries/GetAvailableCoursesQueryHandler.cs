using AutoMapper;
using LMS.Domain.Repositories;
using LMS.Domain.Constants;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LMS.Core.Queries.Courses.GetAvailableCoursesQueries;

public class GetAvailableCoursesQueryHandler : IRequestHandler<GetAvailableCoursesQuery, PaginationResult<GetAvailableCoursesDto>>
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

    public async Task<PaginationResult<GetAvailableCoursesDto>> Handle(GetAvailableCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
            {
                throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
            }

            _logger.LogInformation("Handling GetAvailableCoursesQuery with SearchString: {SearchString}, SortBy: {SortBy}, Order: {Order}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.SearchString, request.SortBy, request.Order, request.PageNumber, request.PageSize);

            var searchString = request.SearchString;
            Expression<Func<Course, bool>> predicate = c => c.CourseStatus == CourseStatus.Approved;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                predicate = c => c.CourseStatus == CourseStatus.Approved
                    && (c.Name.Contains(searchString) || c.Code.Contains(searchString));
            }

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
                return new PaginationResult<GetAvailableCoursesDto>(request.PageNumber, request.PageSize, 0, new List<GetAvailableCoursesDto>());
            }

            var courses = await _unitOfWork.Courses.FilterAsync(
                predicate,
                orderBy,
                isDescending,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                new[] { "Instructor" });

            var dto = _mapper.Map<List<GetAvailableCoursesDto>>(courses);

            return new PaginationResult<GetAvailableCoursesDto>(request.PageNumber, request.PageSize, totalResult, dto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling GetAvailableCoursesQuery");
            throw;
        }
    }
}
