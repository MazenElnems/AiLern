using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Courses.Commands.DeleteEnrollment;

public class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEnrollmentCommandHandler> _logger;

    public DeleteEnrollmentCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteEnrollmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId);
        if (enrollment == null)
            return Result.Failure(DomainErrors.Enrollment.NotFound($"{{{request.CourseId}, {request.StudentId}}}"));

        _logger.LogInformation("Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                    request.CourseId, request.StudentId);

        _unitOfWork.Enrollments.Delete(enrollment);

        try
        {
            var effectedRows = await _unitOfWork.CommitAsync();

            if (effectedRows < 1)
                return Result.Failure(DomainErrors.Common.BusinessRule("Enrollment.DeleteFailed", "Enrollment deletion failed."));

            _logger.LogInformation("Enrollment with course ID {CourseId} and student ID {StudentId} Deleted successfully",
                                    request.CourseId, request.StudentId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                    request.CourseId, request.StudentId);
            throw;
        }
    }
}
