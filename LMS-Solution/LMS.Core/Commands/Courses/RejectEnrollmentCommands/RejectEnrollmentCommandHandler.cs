using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.RejectEnrollmentCommands;

public class RejectEnrollmentCommandHandler : IRequestHandler<RejectEnrollmentCommand, string>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<RejectEnrollmentCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public RejectEnrollmentCommandHandler(ICourseRepository courseRepository, ILogger<RejectEnrollmentCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _courseRepository = courseRepository;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<string> Handle(RejectEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _courseRepository.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId},{request.StudentId}}}");

            if (enrollment.Status == EnrollmentStatus.Rejected)
                return request.Reason;

            if (enrollment.Status == EnrollmentStatus.Approved)
                throw new CourseUpdateException("Can't reject approved course");

            _logger.LogInformation("Rejecting enrollment with course ID {CourseId} , and student ID {StudentId} ",
                          request.CourseId, request.StudentId);

            enrollment.Status = EnrollmentStatus.Rejected;
            await _courseRepository.CommitAsync();

            _logger.LogInformation("enrollment with course ID {CourseId} , and student ID {StudentId} Rejected successfully",
                           request.CourseId, request.StudentId);

            return request.Reason;
        }
        catch (CourseUpdateException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while Rejecting enrollment with course ID {CourseId} , and student ID {StudentId} ",
           request.CourseId, request.StudentId);
            throw ;
        }

    }
}
