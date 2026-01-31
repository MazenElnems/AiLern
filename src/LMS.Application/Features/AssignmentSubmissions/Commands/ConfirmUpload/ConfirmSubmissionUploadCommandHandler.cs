using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload
{
    public class ConfirmSubmissionUploadCommandHandler : IRequestHandler<ConfirmSubmissionUploadCommand, Result>
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWasabiService _wasabiService;


        public ConfirmSubmissionUploadCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IWasabiService wasabiService)
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _wasabiService = wasabiService;
        }

        public async Task<Result> Handle(ConfirmSubmissionUploadCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var submission = await _unitOfWork.Submissions.GetAsync(s => s.Id == request.SubmissionId, [nameof(AssignmentSubmission.Files)]);

            if (submission == null)
                return Result.Failure(DomainErrors.Submission.NotFound(request.SubmissionId.ToString()));

            if(submission.StudentId != user.Id)
                return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to confirm files in this submission."));

            foreach(var file in submission.Files)
            {
                var exists = await _wasabiService.FileExists(file.StoragePath);

                if (!exists)
                    return Result.Failure(DomainErrors.Storage.FileMissing);

                file.UploadStatus = UploadStatus.Completed;
            }

            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
    }
}
