using AutoMapper;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.Exceptions;
using LMS.Shared.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetByIdQueries;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, GetCourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<GetCourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id)
            ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());
        var dto = _mapper.Map<GetCourseDto>(course);
        return dto;
    }
}
