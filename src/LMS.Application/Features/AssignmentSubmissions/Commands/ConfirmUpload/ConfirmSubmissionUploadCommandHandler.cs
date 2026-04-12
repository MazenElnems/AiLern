using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Application.Contracts.ExternalServices;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload;

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

        var submission = await _unitOfWork.AssignmentSubmissions.GetAsync(s => s.Id == request.SubmissionId, [nameof(AssignmentSubmission.Files)]);

        if (submission == null)
            return Result.Failure(DomainErrors.AssignmentSubmission.NotFound(request.SubmissionId.ToString()));

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
