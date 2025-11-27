using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.RejectCommands;

internal class RejectCourseCommandHandler : IRequestHandler<RejectCourseCommand, string>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<RejectCourseCommandHandler> _logger;

    public RejectCourseCommandHandler(ICourseRepository courseRepository, ILogger<RejectCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<string> Handle(RejectCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            if (course.CourseStatus == CourseStatus.Rejected)
                return request.Reason; 

            if (course.CourseStatus == CourseStatus.Approved)
                throw new CourseUpdateException("Can't reject approved course");

            _logger.LogInformation("Rejecting course with ID {CourseId}", request.Id);
            course.CourseStatus = CourseStatus.Rejected;
            var rowsEffect = await _courseRepository.CommitAsync();

            if (rowsEffect == 0)
                _logger.LogWarning("Course status is not updatde");

            else
                _logger.LogInformation("Course with ID {CourseId} rejected successfully", request.Id);
                
            return request.Reason;
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
