using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.CreateEnrollment;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, Result>
{
    private readonly ILogger<EnrollCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public EnrollCourseCommandHandler(ILogger<EnrollCourseCommandHandler> logger, IUserContext userContext, IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _logger = logger;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != user.Id)
            return DomainErrors.Course.NotOwned;

        var student = await _unitOfWork.Users.GetAsync(u => u.Email == request.StudentEmail);

        if (student == null || student.Role != UserRoles.Student)
            return DomainErrors.Common.BusinessRule( "Enrollment Failed",
                $"No student found with the email '{request.StudentEmail}'.");

        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, student.Id);

        if (isEnrolled)
            return DomainErrors.Course.AlreadyEnrolled;

        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = course.Id,
            EnrolledAt = DateTime.UtcNow
        };

        course.Enrollments.Add(enrollment);

        try
        {
            await _unitOfWork.CommitAsync();
            await _notificationService.NotifyUserWithEmailAsync(
                student.Id,
                $"Welcome to {course.Name}",
                $"You're now enrolled in \"{course.Name}\". Explore the course and begin your learning journey.",
                NotificationType.EnrolledInNewCourse,
                $"https://www.ailern.me/courses/{course.Id}",
                "Go to Course"
            );
            return Result.Success("Student enrolled successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling in course with ID {CourseId} by student with Email {StudentEmail}", request.CourseId, request.StudentEmail);
            throw;
        }

    }
}
