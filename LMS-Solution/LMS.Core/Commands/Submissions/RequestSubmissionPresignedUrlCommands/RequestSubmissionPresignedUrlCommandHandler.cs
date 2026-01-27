using LMS.Core.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Submissions.RequestSubmissionPresignedUrlCommands
{
    internal class RequestSubmissionPresignedUrlCommandHandler : IRequestHandler<RequestSubmissionPresignedUrlCommand, PreSignedUrlResponse>
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWasabiService _wasabiService;
        private readonly static int MaxFileSizeInBytes = 10 * 1024 * 1024;
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

        public RequestSubmissionPresignedUrlCommandHandler(IUserContext user, IUnitOfWork unitOfWork, IWasabiService wasabiService)
        {
            _userContext = user;
            _unitOfWork = unitOfWork;
            _wasabiService = wasabiService;
        }

        public async Task<PreSignedUrlResponse> Handle(RequestSubmissionPresignedUrlCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var submission = await _unitOfWork.Submissions.GetAsync(ass => ass.Id == request.SubmissionId, [nameof(Assignment)]);


            if (!user.IsInRole(UserRoles.Student))
                throw new UnauthorizedAccessException("you are not student.");

            if (submission==null)
                throw new ResourceNotFoundException(nameof(AssignmentSubmission), request.SubmissionId.ToString());

            //var course = submission.Assignment;
            var course = await _unitOfWork.Courses.GetAsync(c => c.Id == submission.Assignment.CourseId);
            var enrollment = await _unitOfWork.Enrollments.AnyAsync(e => e.Course_id == course.Id && e.Student_id == user.Id && e.Status == EnrollmentStatus.Approved);

            if (!enrollment)
                throw new UnauthorizedAccessException("You are not enrolled in this course.");

            if (submission.StudentId != user.Id)
                throw new UnauthorizedAccessException("this submission is nor related to you.");

            if (request.Files.Count > 10)
                throw new ValidationException("You can upload a maximum of 10 files.");

            var response = new PreSignedUrlResponse
            {
                PresignedUrls = new List<string>()
            };

            var submissionFiles = new List<AssignmentSubmissionFile>();

            foreach (var file in request.Files)
            {
                if (file.FileSize <= 0 || string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.ContentType))
                    throw new ValidationException("Invalid file metadata provided.");

                if (!allowedContentTypes.Contains(file.ContentType))
                    throw new ValidationException($"File type {file.ContentType} is not allowed.");

                if (file.FileSize > MaxFileSizeInBytes)
                    throw new ValidationException("File size exceeds the maximum allowed limit of 10 MB.");

                var extension = Path.GetExtension(file.FileName);

                if (string.IsNullOrEmpty(extension))
                    throw new ValidationException("Invalid file name.");

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

            return response;


        }
    }
}
