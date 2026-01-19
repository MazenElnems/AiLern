using LMS.Core.CurrentUser;
using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.CourseEnrollmentsCommands;

public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand>
{
    private readonly ILogger<EnrollCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly ICourseRepository _courseRepository;

    public EnrollCourseCommandHandler(ILogger<EnrollCourseCommandHandler> logger, IUserContext userContext, ICourseRepository courseRepository)
    {
        _logger = logger;
        _userContext = userContext;
        _courseRepository = courseRepository;
    }

    public async Task Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentStudentId = _userContext.GetCurrentUser().Id;

            var course = await _courseRepository.GetByIdAsync(request.CourseId)
                ?? throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

            if(course.CourseStatus != CourseStatus.Approved)
                throw new CourseEnrollmentException("Cannot enroll in a course that is not approved.");
            
            if(await _courseRepository.GetEnrollmentByIdAsync(currentStudentId, request.CourseId) != null)
                throw new CourseEnrollmentException("Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                Status = EnrollmentStatus.Pending,
                Student_id = currentStudentId,
            };

            course.Enrollments.Add(enrollment);

            await _courseRepository.CommitAsync();
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
