using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Courses.UpdateCommands;

public class UpdateCourseDetailsCommandHandler : IRequestHandler<UpdateCourseDetailsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseDetailsCommandHandler> _logger;

    public UpdateCourseDetailsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCourseDetailsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateCourseDetailsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id)
                ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

            course.Name = request.Name;
            course.Description = request.Description;
            course.Code = request.Code;

            await _unitOfWork.CommitAsync();
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
