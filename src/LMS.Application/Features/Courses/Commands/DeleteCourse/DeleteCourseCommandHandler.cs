using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCourseCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public ILogger<DeleteCourseCommandHandler> Logger => _logger;

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result.Failure(DomainErrors.Course.NotFound(request.Id));

        Logger.LogInformation("Deleting course with ID {CourseId}", request.Id);
        _unitOfWork.Courses.Delete(course);

        try
        {
            await _unitOfWork.CommitAsync();
            Logger.LogInformation("Course with ID {CourseId} deleted successfully", request.Id);
            return Result.Success();
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
