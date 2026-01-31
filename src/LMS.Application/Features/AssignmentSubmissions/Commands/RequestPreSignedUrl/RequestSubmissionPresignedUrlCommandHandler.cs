using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.RequestPreSignedUrl;

public class RequestSubmissionPresignedUrlCommandHandler : IRequestHandler<RequestSubmissionPresignedUrlCommand, Result<PreSignedUrlResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<RequestSubmissionPresignedUrlCommandHandler> _logger;

    public RequestSubmissionPresignedUrlCommandHandler(IUserContext user, IUnitOfWork unitOfWork, IWasabiService wasabiService, ILogger<RequestSubmissionPresignedUrlCommandHandler> logger)
    {
        _userContext = user;
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task<Result<PreSignedUrlResponse>> Handle(RequestSubmissionPresignedUrlCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var submission = await _unitOfWork.Submissions.GetAsync(ass => ass.Id == request.SubmissionId, [nameof(Assignment)]);

            if (submission == null)
                return Result<PreSignedUrlResponse>.Failure(DomainErrors.Submission.NotFound(request.SubmissionId.ToString()));

            var course = await _unitOfWork.Courses.GetAsync(c => c.Id == submission.Assignment.CourseId);
            var enrollment = await _unitOfWork.Enrollments.AnyAsync(e => e.Course_id == course.Id && e.Student_id == user.Id && e.Status == EnrollmentStatus.Approved);

            if (!enrollment)
                return Result<PreSignedUrlResponse>.Failure(DomainErrors.Submission.NotEnrolled);

            if (submission.StudentId != user.Id)
                return Result<PreSignedUrlResponse>.Failure(DomainErrors.Common.Forbidden("This submission is not related to you."));

            var response = new PreSignedUrlResponse
            {
                PresignedUrls = new List<string>()
            };

            var submissionFiles = new List<AssignmentSubmissionFile>();

            foreach (var file in request.Files)
            {
                var key = $"courses/{course.Name}/assignments/{submission.AssignmentId}/submissions/{Guid.NewGuid()}_{file.FileName}";
                var presignedUrl = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 15);

                submissionFiles.Add(new AssignmentSubmissionFile
                {
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    StoragePath = key,
                    AssignmentSubmissionId = request.SubmissionId,
                    UploadStatus = UploadStatus.Pending
                });

                response.PresignedUrls.Add(presignedUrl);
            }

            submission.Files.AddRange(submissionFiles);
            await _unitOfWork.CommitAsync();

            return Result<PreSignedUrlResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URLs for submission ID {SubmissionId}", request.SubmissionId);
            throw;
        }
    }
}
