using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;

public class RequestMaterialPresignedUrlCommandHandler : IRequestHandler<RequestMaterialPresignedUrlCommand, Result<List<string>>>
{
    private readonly ILogger<RequestMaterialPresignedUrlCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundJobService _backgroundService;

    public RequestMaterialPresignedUrlCommandHandler(ILogger<RequestMaterialPresignedUrlCommandHandler> logger, IUnitOfWork unitOfWork, IPermissionService permissionService, IWasabiService wasabiService, IBackgroundJobService backgroundService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _wasabiService = wasabiService;
        _backgroundService = backgroundService;
    }

    public async Task<Result<List<string>>> Handle(RequestMaterialPresignedUrlCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.SectionId);
            if (!sectionResult.IsSuccess) return Result<List<string>>.Failure(sectionResult.Error!);
            var section = sectionResult.Value!;
            var course = section.Course;

            List<string> response = new();
            List<string> keys = new();

            var orderIndex = section.GetMaxFileOrderIndexAsync();

            foreach (var file in request.Files)
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
