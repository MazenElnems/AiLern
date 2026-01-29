using LMS.Application.CurrentUser;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Application.Commands.Courses.CourseEnrollmentsCommands;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand>
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

    public async Task Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentStudentId = _userContext.GetCurrentUser().Id;

            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId)
                ?? throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

            if(course.CourseStatus != CourseStatus.Approved)
                throw new CourseEnrollmentException("Cannot enroll in a course that is not approved.");
            
            if(await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(currentStudentId, request.CourseId) != null)
                throw new CourseEnrollmentException("Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                Status = EnrollmentStatus.Pending,
                Student_id = currentStudentId,
            };

            course.Enrollments.Add(enrollment);

            await _unitOfWork.CommitAsync();
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(CourseEnrollmentException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling in course with ID {CourseId} by student with ID {StudentId}", request.CourseId, _userContext.GetCurrentUser().Id);
            throw;   
        }
    }
}
