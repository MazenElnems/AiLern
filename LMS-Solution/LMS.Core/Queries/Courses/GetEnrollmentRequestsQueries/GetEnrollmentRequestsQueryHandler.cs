using LMS.Domain.Repositories;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Queries.Courses.GetEnrollmentRequestsQueries;

public class GetEnrollmentRequestsQueryHandler : IRequestHandler<GetEnrollmentRequestsQuery, List<GetEnrollmentRequestsDto>>
{
    private readonly ILogger<GetEnrollmentRequestsQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetEnrollmentRequestsQueryHandler(ILogger<GetEnrollmentRequestsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GetEnrollmentRequestsDto>> Handle(GetEnrollmentRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetEnrollmentRequestsQuery for Course ID: {CourseId}", request.CourseId);
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId)
                ?? throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

            if (course.CourseStatus != CourseStatus.Approved)
                throw new ForbiddenException("Can't view erollment requests");

            var dto = await _unitOfWork.Enrollments.GetEnrollmentRequestsAsync(request.CourseId);

            return dto;
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(ForbiddenException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting enrollment requests for course with ID {CourseId}", request.CourseId);
            throw;
        }

    }
}
