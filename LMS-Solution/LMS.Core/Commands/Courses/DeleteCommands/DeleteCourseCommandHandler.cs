using LMS.Domin.Repositories;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.DeleteCommands;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;

    public DeleteCourseCommandHandler(ICourseRepository courseRepository, ILogger<DeleteCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public ILogger<DeleteCourseCommandHandler> Logger => _logger;

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            Logger.LogInformation("Deleting course with ID {CourseId}", request.Id);
            await _courseRepository.RemoveAsync(course);
            Logger.LogInformation("Course with ID {CourseId} deleted successfully", request.Id);
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
