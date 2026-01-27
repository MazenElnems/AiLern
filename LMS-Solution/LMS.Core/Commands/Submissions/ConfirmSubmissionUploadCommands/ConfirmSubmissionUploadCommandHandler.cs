using LMS.Core.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Submissions.ConfirmSubmissionUploadCommands
{
    public class ConfirmSubmissionUploadCommandHandler : IRequestHandler<ConfirmSubmissionUploadCommand>
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

        public async Task Handle(ConfirmSubmissionUploadCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var submission = await _unitOfWork.Submissions.GetAsync(s => s.Id == request.SubmissionId, [nameof(AssignmentSubmission.Files)]);

            if (submission == null)
                throw new ResourceNotFoundException(nameof(AssignmentSubmission), request.SubmissionId.ToString());

            if(submission.StudentId != user.Id)
                throw new UnauthorizedAccessException("You do not have permission to confirm files in this submission.");

            foreach(var file in submission.Files)
            {
                var exists = await _wasabiService.FileExists(file.StoragePath);

                if (!exists)
                    throw new ValidationException("File does not exist in storage.");

                file.UploadStatus = Domain.Enums.UploadStatus.Completed;
            }

            await _unitOfWork.CommitAsync();

        }
    }
}
