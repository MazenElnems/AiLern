using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload;

public class ConfirmSubmissionUploadCommandHandler : IRequestHandler<ConfirmSubmissionUploadCommand, Result>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWasabiService _wasabiService;

    public ConfirmSubmissionUploadCommandHandler(IPermissionService permissionService, IUnitOfWork unitOfWork, IWasabiService wasabiService)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(ConfirmSubmissionUploadCommand request, CancellationToken cancellationToken)
    {
        var submissionResult = await _permissionService.AuthorizeStudentAccessToSubmissionAsync(request.SubmissionId);
        if (!submissionResult.IsSuccess) return Result.Failure(submissionResult.Error!);
        var submission = submissionResult.Value!;

        var submissionWithFiles = await _unitOfWork.AssignmentSubmissions.GetAsync(
            s => s.Id == request.SubmissionId,
            [nameof(AssignmentSubmission.Files)]);

        foreach (var file in submissionWithFiles!.Files)
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
