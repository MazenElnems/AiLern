using AutoMapper;
using LMS.Core.DTOs.Courses;
using LMS.Domin.Contracts;
using LMS.Domin.Enums;
using MediatR;

namespace LMS.Core.Queries.Courses.GetApprovedQueries;

public class GetApprovedCoursesQueryHandler : IRequestHandler<GetApprovedCoursesQuery, List<GetApprovedCoursesDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetApprovedCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<GetApprovedCoursesDto>> Handle(GetApprovedCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetPagedCoursesWithFilterAsync(
            c => c.CourseStatus == CourseStatus.Approved,
            request.SearchString!,
            request.SortBy!,
            request.Order!,
            request.PageNumber,
            request.PageSize
        );
        var dto = _mapper.Map<List<GetApprovedCoursesDto>>(courses);
        return dto;
    }
}
