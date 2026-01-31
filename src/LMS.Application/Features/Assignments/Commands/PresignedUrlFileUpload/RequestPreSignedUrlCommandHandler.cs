using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Assignments.Commands.PresignedUrlFileUpload;

public class RequestPreSignedUrlCommandHandler : IRequestHandler<RequestPreSignedUrlCommand, Result<PreSignedUrlResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<RequestPreSignedUrlCommandHandler> _logger;

    public RequestPreSignedUrlCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService, ILogger<RequestPreSignedUrlCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task<Result<PreSignedUrlResponse>> Handle(RequestPreSignedUrlCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userContext.GetCurrentUser().Id;

            var assignment = await _unitOfWork.Assignments
                .GetAsync(a => a.Id == request.AssignmentId, [nameof(Course)]);

            if (assignment == null)
                return Result<PreSignedUrlResponse>.Failure(DomainErrors.Assignment.NotFound(request.AssignmentId));

            var course = assignment.Course;

            if (course.InstructorId != userId)
                return Result<PreSignedUrlResponse>.Failure(
                    DomainErrors.Common.Forbidden("You do not have permission to request pre-signed URLs for this assignment."));

            var response = new PreSignedUrlResponse
            {
                PresignedUrls = new List<string>()
            };

            var assignmentFiles = new List<AssignmentFile>();

            foreach (var file in request.Files)
            {
                var key = $"courses/{course.Name}/assignments/{request.AssignmentId}/{Guid.NewGuid()}_{file.FileName}";
                var preSignedUrl = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 2);

                assignmentFiles.Add(new AssignmentFile
                {
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    StoragePath = key,
                    UploadStatus = UploadStatus.Pending,
                    AssignmentId = request.AssignmentId
                });

                response.PresignedUrls.Add(preSignedUrl);
            }

            assignment.Files.AddRange(assignmentFiles); 
            await _unitOfWork.CommitAsync();

            return Result<PreSignedUrlResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an unexpected error occurred while processing RequestPreSignedUrlCommandHandler for assignment ID {AssignmentId}", request.AssignmentId);
            throw;
        }
    }
}
