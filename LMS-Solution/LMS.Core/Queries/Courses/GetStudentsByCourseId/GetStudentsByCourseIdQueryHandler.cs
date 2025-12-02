using AutoMapper;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs.Students;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Queries.Courses.GetStudentsByCourseId;

public class GetStudentsByCourseIdQueryHandler : IRequestHandler<GetStudentsByCourseIdQuery, List<GetStudentsByCourseIdDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentsByCourseIdQueryHandler> _logger;

    public GetStudentsByCourseIdQueryHandler(ICourseRepository courseRepository, IMapper mapper, ILogger<GetStudentsByCourseIdQueryHandler> logger)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<GetStudentsByCourseIdDto>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // select course from db
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            var students = await _courseRepository.GetStudentsByCourseIdAsync(request.Id);

            var dto = _mapper.Map<List<GetStudentsByCourseIdDto>>(students);

            return dto;
        }
        catch (ResourceNotFoundException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError("an exception happen while getting students in course with id: {CourseId}", request.Id);
            throw;
        }

    }
}
