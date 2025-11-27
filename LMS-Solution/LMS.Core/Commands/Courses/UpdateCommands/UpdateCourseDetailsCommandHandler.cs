using LMS.Core.CustomExceptions;
using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.UpdateCommands;

public class UpdateCourseDetailsCommandHandler : IRequestHandler<UpdateCourseDetailsCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<UpdateCourseDetailsCommandHandler> _logger;

    public UpdateCourseDetailsCommandHandler(ICourseRepository courseRepository, ILogger<UpdateCourseDetailsCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task Handle(UpdateCourseDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            course.Name = request.Name;
            course.Description = request.Description;
            course.Code = request.Code;

            await _courseRepository.CommitAsync();
        }
        catch(ResourceNotFoundException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating course details for CourseId: {CourseId}", request.Id);
            throw;
        }
    }
}
