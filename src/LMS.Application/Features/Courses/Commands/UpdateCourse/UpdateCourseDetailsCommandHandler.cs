using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommandHandler : IRequestHandler<UpdateCourseDetailsCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseDetailsCommandHandler> _logger;
    private readonly IWasabiService _wasabiService;

    public UpdateCourseDetailsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCourseDetailsCommandHandler> logger, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _wasabiService = wasabiService;
    }

    public async Task<Result<string>> Handle(UpdateCourseDetailsCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return DomainErrors.Course.NotFound(request.Id);

        if (request.Image != null && !string.IsNullOrEmpty(course.ImageStoragePath))
        {
            var isExist = await _wasabiService.FileExists(course.ImageStoragePath, false);
            if (isExist)
            {
                await _wasabiService.DeleteFileAsync(course.ImageStoragePath, cancellationToken, false);
            }
        }
        string? key = null;
        string? url = null;
        if (request.Image != null && !request.Image.ContentType.StartsWith("image/"))
        {
            return DomainErrors.Common.BusinessRule("Invalid Image", "The uploaded file must be an image.");
        }
        if (request.Image != null)
        {
            key = $"courses/{request.Code}/photo/{Guid.NewGuid()}.{request.Image.FileName.Split('.').Last()}";
            url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, request.Image.ContentType, 15, secret: false);
        }
        course.Name = request.Name;
        course.Description = request.Description;
        course.Code = request.Code;
        course.ImageStoragePath = key;
        try
        {
            await _unitOfWork.CommitAsync();
            return url;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating course details for CourseId: {CourseId}", request.Id);
            throw;
        }
    }
}
