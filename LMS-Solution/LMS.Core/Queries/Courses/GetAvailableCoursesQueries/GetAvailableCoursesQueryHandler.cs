using AutoMapper;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Queries.Courses.GetAvailableCoursesQueries;

public class GetAvailableCoursesQueryHandler : IRequestHandler<GetAvailableCoursesQuery, List<GetAvailableCoursesDto>>
{
    private readonly ILogger<GetAvailableCoursesQueryHandler> _logger;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetAvailableCoursesQueryHandler(ICourseRepository courseRepository, ILogger<GetAvailableCoursesQueryHandler> logger, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<GetAvailableCoursesDto>> Handle(GetAvailableCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetAvailableCoursesQuery with SearchString: {SearchString}, SortBy: {SortBy}, Order: {Order}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.SearchString, request.SortBy, request.Order, request.PageNumber, request.PageSize);

            var courses = await _courseRepository.GetPagedCoursesWithFilterAsync(
                c => c.CourseStatus == CourseStatus.Approved,
                request.SearchString!,
                request.SortBy!,
                request.Order!,
                request.PageNumber,
                request.PageSize
            );

            var dto = _mapper.Map<List<GetAvailableCoursesDto>>(courses);
            return dto;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling GetAvailableCoursesQuery");
            throw;
        }
    }
}
