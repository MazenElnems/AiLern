using AutoMapper;
using LMS.Domin.Repositories;
using LMS.Domin.Constants;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
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
