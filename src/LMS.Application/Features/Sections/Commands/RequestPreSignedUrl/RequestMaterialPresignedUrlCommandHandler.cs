using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;

public class RequestMaterialPresignedUrlCommandHandler : IRequestHandler<RequestMaterialPresignedUrlCommand, Result<List<string>>>
{
    private readonly ILogger<RequestMaterialPresignedUrlCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundService _backgroundService;

    public RequestMaterialPresignedUrlCommandHandler(ILogger<RequestMaterialPresignedUrlCommandHandler> logger, IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService, IBackgroundService backgroundService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
        _backgroundService = backgroundService;
    }

    public async Task<Result<List<string>>> Handle(RequestMaterialPresignedUrlCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var section = await _unitOfWork.Sections.GetAsync(s => s.Id == request.SectionId,
                includeProperties: [nameof(Section.Course)]);

            if (section == null)
                return DomainErrors.Section.NotFound(request.SectionId);

            var course = section.Course;

            if (user.Id != course.InstructorId)
                return DomainErrors.Common.Forbidden("You do not have permission to request pre-signed URLs for this assignment.");

            List<string> response = new();
            List<string> keys = new();

            var orderIndex = await _unitOfWork.MaterialFiles.GetMaxOrderIndexAsync(request.SectionId);

            foreach(var file in request.Files)
            {
                var key = $"courses/{course.Name}/Materials/{Guid.NewGuid()}_{file.FileName}";
                var preSignedUrl = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 2);

                section.MaterialFiles.Add(new MaterialFile
                {
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    StoragePath = key,
                    UploadDate = DateTime.UtcNow,
                    UploadStatus = UploadStatus.Pending,
                    SectionId = request.SectionId,
                    FileSize = file.FileSize,
                    OrderIndex = ++orderIndex
                });

                response.Add(preSignedUrl);
                keys.Add(key);
            }

            await _unitOfWork.CommitAsync();

            _backgroundService.Schedule<IConfirmUploadedFilesJob>(
                job => job.ExecuteAsync(keys),
                TimeSpan.FromMinutes(2)
            );
            return response;
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URLs for submission ID {SubmissionId}", request.SectionId);
            throw;
        }
    }
}
