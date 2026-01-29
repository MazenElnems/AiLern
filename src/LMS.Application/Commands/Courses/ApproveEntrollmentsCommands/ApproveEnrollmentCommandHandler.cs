using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Application.Commands.Courses.ApproveEntrollmentsCommands;

public class ApproveEnrollmentCommandHandler : IRequestHandler<ApproveEnrollmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveEnrollmentCommandHandler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApproveEnrollmentCommandHandler(IUnitOfWork unitOfWork, ILogger<ApproveEnrollmentCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(ApproveEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId}, {request.StudentId}}}");

            if (enrollment.Status == EnrollmentStatus.Approved)
                return;

            if (enrollment.Status == EnrollmentStatus.Rejected)
                throw new CourseUpdateException("Can't approve already rejected course");


            _logger.LogInformation("Approving enrollment with course ID {CourseId} and student ID {StudentId}", 
                                        request.CourseId,request.StudentId);

            enrollment.Status = EnrollmentStatus.Approved;
            await _unitOfWork.CommitAsync();

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
