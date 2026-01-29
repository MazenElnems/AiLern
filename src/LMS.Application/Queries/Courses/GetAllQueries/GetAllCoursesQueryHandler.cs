using AutoMapper;
using LMS.Domain.Repositories;
using LMS.Domain.Constants;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Queries.Courses.GetAllQueries;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, PaginationResult<GetAllCoursesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllCoursesQueryHandler> _logger;

    public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCoursesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginationResult<GetAllCoursesDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
        }

        try
        {
            var searchString = request.SearchString;
            Expression<Func<Course, bool>> predicate = c => true;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                predicate = c => c.Name.Contains(searchString) || c.Code.Contains(searchString);
            }

            var sortBy = request.SortBy;
            var order = request.Order?.ToLower();
            var isDescending = order != SortOrderOptions.ASC;

            Expression<Func<Course, object>> orderBy = sortBy?.ToLower() switch
            {
                var s when s == CourseSortByOptions.Name => c => c.Name,
                var s when s == CourseSortByOptions.CreatedAt => c => c.CreatedAt,
                _ => c => c.CreatedAt
            };

            var totalResult = await _unitOfWork.Courses.CountAsync(predicate);

            if(totalResult == 0)
            {
                return new PaginationResult<GetAllCoursesDto>(request.PageNumber, request.PageSize, 0, new List<GetAllCoursesDto>());
            }

            var courses = await _unitOfWork.Courses.FilterAsync(
                predicate,
                orderBy,
                isDescending,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                new[] { "Instructor" });

            var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);
            return new PaginationResult<GetAllCoursesDto>(request.PageNumber, request.PageSize, totalResult, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving courses.");
            throw;
        }
    }
}
