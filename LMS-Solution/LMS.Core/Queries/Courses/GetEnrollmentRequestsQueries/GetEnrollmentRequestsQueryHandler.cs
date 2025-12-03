using LMS.Domin.Contracts;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Queries.Courses.GetEnrollmentRequestsQueries;

public class GetEnrollmentRequestsQueryHandler : IRequestHandler<GetEnrollmentRequestsQuery, List<GetEnrollmentRequestsDto>>
{
    private readonly ILogger<GetEnrollmentRequestsQueryHandler> _logger;
    private readonly ICourseRepository _courseRepository;
    public GetEnrollmentRequestsQueryHandler(ILogger<GetEnrollmentRequestsQueryHandler> logger, ICourseRepository courseRepository)
    {
        _logger = logger;
        _courseRepository = courseRepository;
    }

    public async Task<List<GetEnrollmentRequestsDto>> Handle(GetEnrollmentRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetEnrollmentRequestsQuery for Course ID: {CourseId}", request.CourseId);
            var course = await _courseRepository.GetByIdAsync(request.CourseId)
                ?? throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

            if (course.CourseStatus != CourseStatus.Approved)
                throw new ForbiddenException("Can't view erollment requests");

            var dto = await _courseRepository.GetEnrollmentRequestsAsync(request.CourseId);

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
