using AutoMapper;
using LMS.Domin.Repositories;
using LMS.Domin.Constants;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;
using MediatR;
using System.Linq.Expressions;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, PaginationResult<GetAllCoursesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginationResult<GetAllCoursesDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var searchString = request.SearchString;
        Expression<Func<Course, bool>> predicate = c => true;

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            predicate = c => c.Name.Contains(searchString) || c.Code.Contains(searchString);
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
        
        var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);

        return new PaginationResult<GetAllCoursesDto>(request.PageNumber, request.PageSize, totalResult, dto);
    }
}
