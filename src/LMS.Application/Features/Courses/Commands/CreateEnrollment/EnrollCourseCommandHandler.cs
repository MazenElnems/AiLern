using LMS.Application.CurrentUser;
using LMS.Domain.Repositories;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;

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
        var currentStudentId = _userContext.GetCurrentUser().Id;

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        
        
        if(await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(currentStudentId, request.CourseId) != null)
            return DomainErrors.Course.AlreadyEnrolled;

        var enrollment = new Enrollment
        {
            Status = EnrollmentStatus.Pending,
            StudentId = currentStudentId,
        };

        course.Enrollments.Add(enrollment);

        try
        {
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling in course with ID {CourseId} by student with ID {StudentId}", request.CourseId, currentStudentId);
            throw;
        }
    }
}
