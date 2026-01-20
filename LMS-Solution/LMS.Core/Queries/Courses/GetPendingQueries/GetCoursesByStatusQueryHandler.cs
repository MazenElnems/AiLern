using AutoMapper;
using LMS.Domin.Constants;
using LMS.Domin.Repositories;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using MediatR;
using System.Linq.Expressions;

namespace LMS.Core.Queries.Courses.GetPendingQueries;


public class GetCoursesByStatusQueryHandler : IRequestHandler<GetCoursesByStatusQuery, List<GetCourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCoursesByStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GetCourseDto>> Handle(GetCoursesByStatusQuery request, CancellationToken cancellationToken)
    {
        var status = Enum.Parse<CourseStatus>(request.Status, true);
        var searchString = request.SearchString;
        Expression<Func<Course, bool>> predicate = c => c.CourseStatus == status;

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            predicate = c => c.CourseStatus == status
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

        var courses = await _unitOfWork.Courses.FilterAsync(
            predicate,
            orderBy,
            isDescending,
            (request.PageNumber - 1) * request.PageSize,
            request.PageSize,
            new[] { "Instructor" });

        var dto = _mapper.Map<List<GetCourseDto>>(courses);
      
        return dto;
    }
}
