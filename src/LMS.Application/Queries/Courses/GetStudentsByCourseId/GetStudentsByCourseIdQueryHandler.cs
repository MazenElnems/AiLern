using AutoMapper;
using LMS.Domain.Repositories;
using LMS.Domain.DTOs.Students;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Queries.Courses.GetStudentsByCourseId;

public class GetStudentsByCourseIdQueryHandler : IRequestHandler<GetStudentsByCourseIdQuery, List<GetStudentsByCourseIdDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentsByCourseIdQueryHandler> _logger;

    public GetStudentsByCourseIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetStudentsByCourseIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<GetStudentsByCourseIdDto>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // select course from db
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            var students = await _unitOfWork.Courses.GetStudentsByCourseIdAsync(request.Id);

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
