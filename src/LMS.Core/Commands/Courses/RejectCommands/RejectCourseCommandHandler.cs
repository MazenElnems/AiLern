using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Core.Commands.Courses.RejectCommands;

internal class RejectCourseCommandHandler : IRequestHandler<RejectCourseCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectCourseCommandHandler> _logger;

    public RejectCourseCommandHandler(IUnitOfWork unitOfWork, ILogger<RejectCourseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> Handle(RejectCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            if (course.CourseStatus == CourseStatus.Rejected)
                return request.Reason; 

            if (course.CourseStatus == CourseStatus.Approved)
                throw new CourseUpdateException("Can't reject approved course");

            _logger.LogInformation("Rejecting course with ID {CourseId}", request.Id);
            course.CourseStatus = CourseStatus.Rejected;
            var rowsEffect = await _unitOfWork.CommitAsync();

            if (rowsEffect == 0)
                _logger.LogWarning("Course status is not updatde");

            else
                _logger.LogInformation("Course with ID {CourseId} rejected successfully", request.Id);
                
            return request.Reason;
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
