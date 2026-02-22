using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Enums;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Courses.Commands.RejectCourse;

internal class RejectCourseCommandHandler : IRequestHandler<RejectCourseCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectCourseCommandHandler> _logger;

    public RejectCourseCommandHandler(IUnitOfWork unitOfWork, ILogger<RejectCourseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(RejectCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result<string>.Failure(DomainErrors.Course.NotFound(request.Id));

        if (course.CourseStatus == CourseStatus.Rejected)
            return Result<string>.Success(request.Reason);

        if (course.CourseStatus == CourseStatus.Approved)
            return Result<string>.Failure(DomainErrors.Course.RejectApproved);

        _logger.LogInformation("Rejecting course with ID {CourseId}", request.Id);
        course.CourseStatus = CourseStatus.Rejected;

        try
        {
            var rowsEffect = await _unitOfWork.CommitAsync();

            if (rowsEffect == 0)
                _logger.LogWarning("Course status is not updated");
            else
                _logger.LogInformation("Course with ID {CourseId} rejected successfully", request.Id);

            return Result<string>.Success(request.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
