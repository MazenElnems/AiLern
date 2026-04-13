using LMS.Domain.Entities;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Errors;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommandHandler : IRequestHandler<UpdateCourseDetailsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseDetailsCommandHandler> _logger;

    public UpdateCourseDetailsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCourseDetailsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCourseDetailsCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result.Failure(DomainErrors.Course.NotFound(request.Id));

        course.Name = request.Name;
        course.Description = request.Description;
        course.Code = request.Code;

        try
        {
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating course details for CourseId: {CourseId}", request.Id);
            throw;
        }
    }
}
