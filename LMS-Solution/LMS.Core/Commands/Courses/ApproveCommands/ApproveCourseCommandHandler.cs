using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.ApproveCommands;

public class ApproveCourseCommandHandler : IRequestHandler<ApproveCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<ApproveCourseCommandHandler> _logger;


    public ApproveCourseCommandHandler(ICourseRepository courseRepository, ILogger<ApproveCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task Handle(ApproveCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id)
                 ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            if (course.CourseStatus == CourseStatus.Approved)
                return;

            if(course.CourseStatus == CourseStatus.Rejected)
                throw new CourseUpdateException("Can't approve already rejected course");
            
            _logger.LogInformation("Approving course with ID {CourseId}", request.Id);
            course.CourseStatus = CourseStatus.Approved;

            _logger.LogInformation("Course with ID {CourseId} Approved successfully", request.Id);
            await _courseRepository.CommitAsync();
        }
        catch (ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while Approving course with ID {CourseId}", request.Id);
            throw;
        }
    }
}
