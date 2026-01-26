using LMS.Core.CurrentUser;
using LMS.Domain.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Commands.Assignments.RequestPreSignedUrlCommands;

public class RequestPreSignedUrlCommandHandler : IRequestHandler<RequestPreSignedUrlCommand, PreSignedUrlResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly static int MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
    private readonly List<string> allowedContentTypes = new List<string>
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg",
        "image/png",
        "text/plain",
        "application/zip"
    };

    public RequestPreSignedUrlCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
    }

    public async Task<PreSignedUrlResponse> Handle(RequestPreSignedUrlCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var assignment = await _unitOfWork.Assignments
            .GetAsync(a => a.Id == request.AssignmentId, [nameof(Course)]);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.AssignmentId.ToString());

        var course = assignment.Course;

        if (course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to request pre-signed URLs for this assignment.");

        if (request.Files.Count > 10)
            throw new ValidationException("You can upload a maximum of 10 files.");

        var response = new PreSignedUrlResponse
        {
            PresignedUrls = new List<string>()
        };

        var assignmentFiles = new List<AssignmentFile>();

        foreach (var file in request.Files)
        {
            if (file.FileSize <= 0 || string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.ContentType))
                throw new ValidationException("Invalid file metadata provided.");

            if(!allowedContentTypes.Contains(file.ContentType))
                throw new ValidationException($"File type {file.ContentType} is not allowed.");

            if (file.FileSize > MaxFileSizeInBytes)
                throw new ValidationException("File size exceeds the maximum allowed limit of 10 MB.");

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrEmpty(extension))
                throw new ValidationException("Invalid file name.");

            var key = $"courses/{course.Name}/assignments/{request.AssignmentId}/{Guid.NewGuid()}_{file.FileName}";
            var preSignedUrl = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 15);

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

        return response;
    }
}
