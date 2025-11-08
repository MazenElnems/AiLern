using AutoMapper;
using LMS.Core.DTOs.Courses;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<GetAllCoursesDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<GetAllCoursesDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync(request.SortBy, request.Order, request.Status, request.PageNumber, request.PageSize);
        var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);
        return dto;
    }
}
