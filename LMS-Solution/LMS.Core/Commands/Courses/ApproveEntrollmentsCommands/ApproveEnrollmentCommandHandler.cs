using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.ApproveEntrollmentsCommands;

public class ApproveEnrollmentCommandHandler : IRequestHandler<ApproveEnrollmentCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<ApproveEnrollmentCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApproveEnrollmentCommandHandler(ICourseRepository course, ILogger<ApproveEnrollmentCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _courseRepository = course;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(ApproveEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _courseRepository.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId}, {request.StudentId}}}");

            if (enrollment.Status == EnrollmentStatus.Approved)
                return;

            if (enrollment.Status == EnrollmentStatus.Rejected)
                throw new CourseUpdateException("Can't approve already rejected course");


            _logger.LogInformation("Approving enrollment with course ID {CourseId} and student ID {StudentId}", 
                                        request.CourseId,request.StudentId);

            enrollment.Status = EnrollmentStatus.Approved;
            await _courseRepository.CommitAsync();

            _logger.LogInformation("Enrollment with course ID {CourseId} and student ID {StudentId} Approved successfully",
                                        request.CourseId, request.StudentId);
        }
        catch(CourseUpdateException)
        {
            throw;
        }
        catch (ResourceNotFoundException)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"An error occurred while Approving enrollment with course ID {CourseId} and student ID {StudentId}",
                                        request.CourseId, request.StudentId);
        }
    }
}
