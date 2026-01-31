using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Application.Features.Courses.Commands.ApproveEnrollment;

public class ApproveEnrollmentCommandHandler : IRequestHandler<ApproveEnrollmentCommand, Result>
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

    public async Task<Result> Handle(ApproveEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId);
        if (enrollment == null)
            return Result.Failure(DomainErrors.Enrollment.NotFound($"{{{request.CourseId}, {request.StudentId}}}"));

        if (enrollment.Status == EnrollmentStatus.Approved)
            return Result.Success();

        if (enrollment.Status == EnrollmentStatus.Rejected)
            return Result.Failure(DomainErrors.Course.ApproveRejected);

        _logger.LogInformation("Approving enrollment with course ID {CourseId} and student ID {StudentId}", 
                                    request.CourseId,request.StudentId);

        enrollment.Status = EnrollmentStatus.Approved;

        try
        {
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Enrollment with course ID {CourseId} and student ID {StudentId} Approved successfully",
                                    request.CourseId, request.StudentId);
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"An error occurred while Approving enrollment with course ID {CourseId} and student ID {StudentId}",
                                    request.CourseId, request.StudentId);
            throw;
        }
    }
}
