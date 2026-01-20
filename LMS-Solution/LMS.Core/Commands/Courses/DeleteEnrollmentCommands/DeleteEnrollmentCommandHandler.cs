using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.DeleteEnrollmentCommands;

public class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEnrollmentCommandHandler> _logger;

    public DeleteEnrollmentCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteEnrollmentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId}, {request.StudentId}}}");

            _logger.LogInformation("Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                        request.CourseId, request.StudentId);

            _unitOfWork.Enrollments.Delete(enrollment);
            var effectedRows = await _unitOfWork.CommitAsync();

            if (effectedRows < 1)
                throw new Exception();

            _logger.LogInformation("Enrollment with course ID {CourseId} and student ID {StudentId} Deleted successfully",
                                        request.CourseId, request.StudentId);
        }
        catch (CourseUpdateException)
        {
            throw;
        }
        catch (ResourceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                        request.CourseId, request.StudentId);
        }
    }
}
