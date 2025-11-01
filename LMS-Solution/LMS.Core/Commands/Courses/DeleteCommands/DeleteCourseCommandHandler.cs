using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.Exceptions;
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

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            _logger.LogInformation("Deleting course with ID {CourseId}", request.Id);
            await _courseRepository.RemoveAsync(course);
            _logger.LogInformation("Course with ID {CourseId} deleted successfully", request.Id);
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
