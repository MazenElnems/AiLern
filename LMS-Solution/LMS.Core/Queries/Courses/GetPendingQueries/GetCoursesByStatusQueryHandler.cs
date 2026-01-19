using AutoMapper;
using LMS.Domin.Repositories;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Enums;
using MediatR;

namespace LMS.Core.Queries.Courses.GetPendingQueries;


public class GetCoursesByStatusQueryHandler : IRequestHandler<GetCoursesByStatusQuery, List<GetCourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetCoursesByStatusQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<GetCourseDto>> Handle(GetCoursesByStatusQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetPagedCoursesWithFilterAsync(
            c => c.CourseStatus == Enum.Parse<CourseStatus>(request.Status,true),
            request.SearchString,
            request.SortBy!,
            request.Order!,
            request.PageNumber,
            request.PageSize
        );
        var dto = _mapper.Map<List<GetCourseDto>>(courses);
      
        return dto;
    }
}
