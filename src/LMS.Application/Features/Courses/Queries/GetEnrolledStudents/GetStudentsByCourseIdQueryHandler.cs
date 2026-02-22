using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Errors;
using LMS.Application.Features.Students.DTO;

namespace LMS.Application.Features.Courses.Queries.GetEnrolledStudents;

public class GetStudentsByCourseIdQueryHandler : IRequestHandler<GetStudentsByCourseIdQuery, Result<List<GetStudentsByCourseIdDto>>>
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

    public async Task<Result<List<GetStudentsByCourseIdDto>>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // select course from db
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id)
                ;
            if (course == null)
                return Result<List<GetStudentsByCourseIdDto>>.Failure(DomainErrors.Course.NotFound(request.Id));

            var students = await _unitOfWork.Courses.GetStudentsByCourseIdAsync(request.Id);

            var dto = _mapper.Map<List<GetStudentsByCourseIdDto>>(students);

            return Result<List<GetStudentsByCourseIdDto>>.Success(dto);
        }
        catch(Exception ex)
        {
            _logger.LogError("an exception happen while getting students in course with id: {CourseId}", request.Id);
            throw;
        }

    }
}
