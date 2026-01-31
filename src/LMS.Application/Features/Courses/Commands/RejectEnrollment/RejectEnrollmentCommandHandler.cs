using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Common.Enums;

namespace LMS.Application.Features.Courses.Commands.RejectEnrollment;

public class RejectEnrollmentCommandHandler : IRequestHandler<RejectEnrollmentCommand, Result<string>>
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

    public async Task<Result<string>> Handle(RejectEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId);
        if (enrollment == null)
            return Result<string>.Failure(DomainErrors.Enrollment.NotFound($"{{{request.CourseId},{request.StudentId}}}"));

        if (enrollment.Status == EnrollmentStatus.Rejected)
            return Result<string>.Success(request.Reason);

        if (enrollment.Status == EnrollmentStatus.Approved)
            return Result<string>.Failure(DomainErrors.Course.RejectApproved);

        _logger.LogInformation("Rejecting enrollment with course ID {CourseId} , and student ID {StudentId} ",
                      request.CourseId, request.StudentId);

        enrollment.Status = EnrollmentStatus.Rejected;

        try
        {
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("enrollment with course ID {CourseId} , and student ID {StudentId} Rejected successfully",
                           request.CourseId, request.StudentId);

            return Result<string>.Success(request.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while Rejecting enrollment with course ID {CourseId} , and student ID {StudentId} ",
           request.CourseId, request.StudentId);
            throw;
        }

    }
}
