using AutoMapper;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, PaginationResult<GetAllCoursesDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResult<GetAllCoursesDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var (courses, totalResult) = await _courseRepository.GetPagedCourses(
            request.SearchString!,
            request.SortBy!,
            request.Order!,
            request.PageNumber,
            request.PageSize
        );
        
        var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);

        return new PaginationResult<GetAllCoursesDto>(request.PageNumber, request.PageSize, totalResult, dto);
    }
}
