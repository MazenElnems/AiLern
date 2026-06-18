using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;
    private readonly INotificationService _notificationService;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCourseCommandHandler> logger, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notificationService = notificationService;
    }

    public ILogger<DeleteCourseCommandHandler> Logger => _logger;

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result.Failure(DomainErrors.Course.NotFound(request.Id));

        Logger.LogInformation("Deleting course with ID {CourseId}", request.Id);
        _unitOfWork.Courses.Delete(course);

        try
        {
            await _unitOfWork.CommitAsync();
            Logger.LogInformation("Course with ID {CourseId} deleted successfully", request.Id);
            await _notificationService.NotifyUserWithEmailAsync(
                course.InstructorId,
                $"{course.Name}: Course Removed",
                $"Your course \"{course.Name}\" has been removed by the administrator and is no longer available.",
                NotificationType.CourseRemovedByAdmin,
                "https://www.ailern.me/instructor/courses",
                "View My Courses"
            );
            return Result.Success();
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
