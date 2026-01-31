using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Application.Features.Courses.Queries.GetEnrollmentRequests;

public class GetEnrollmentRequestsQueryHandler : IRequestHandler<GetEnrollmentRequestsQuery, Result<List<GetEnrollmentRequestsDto>>>
{
    private readonly ILogger<GetEnrollmentRequestsQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetEnrollmentRequestsQueryHandler(ILogger<GetEnrollmentRequestsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<GetEnrollmentRequestsDto>>> Handle(GetEnrollmentRequestsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetEnrollmentRequestsQuery for Course ID: {CourseId}", request.CourseId);
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId)
                ;
            if (course == null)
                return Result<List<GetEnrollmentRequestsDto>>.Failure(DomainErrors.Course.NotFound(request.CourseId));

            if (course.CourseStatus != CourseStatus.Approved)
                return Result<List<GetEnrollmentRequestsDto>>.Failure(
                    DomainErrors.Common.Forbidden("Can't view enrollment requests."));

            var dto = await _unitOfWork.Enrollments.GetEnrollmentRequestsAsync(request.CourseId);

            return Result<List<GetEnrollmentRequestsDto>>.Success(dto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting enrollment requests for course with ID {CourseId}", request.CourseId);
            throw;
        }

    }
}
