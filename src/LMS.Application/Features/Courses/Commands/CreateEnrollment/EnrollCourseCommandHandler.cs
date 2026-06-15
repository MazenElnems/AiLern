using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Constants;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Courses.Commands.CreateEnrollment;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, Result>
{
    private readonly ILogger<EnrollCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollCourseCommandHandler(ILogger<EnrollCourseCommandHandler> logger, IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
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
            return Result.Success("Student enrolled successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling in course with ID {CourseId} by student with Email {StudentEmail}", request.CourseId, request.StudentEmail);
            throw;
        }

    }
}
