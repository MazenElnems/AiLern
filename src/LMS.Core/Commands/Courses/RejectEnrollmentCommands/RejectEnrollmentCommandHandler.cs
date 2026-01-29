using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Core.Commands.Courses.RejectEnrollmentCommands;

public class RejectEnrollmentCommandHandler : IRequestHandler<RejectEnrollmentCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectEnrollmentCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public RejectEnrollmentCommandHandler(IUnitOfWork unitOfWork, ILogger<RejectEnrollmentCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<string> Handle(RejectEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId},{request.StudentId}}}");

            if (enrollment.Status == EnrollmentStatus.Rejected)
                return request.Reason;

            if (enrollment.Status == EnrollmentStatus.Approved)
                throw new CourseUpdateException("Can't reject approved course");

            _logger.LogInformation("Rejecting enrollment with course ID {CourseId} , and student ID {StudentId} ",
                          request.CourseId, request.StudentId);

            enrollment.Status = EnrollmentStatus.Rejected;
            await _unitOfWork.CommitAsync();

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
