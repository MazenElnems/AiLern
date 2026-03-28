using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.ConfirmFileUpload;

public class ConfirmAssignmentUploadCommandHandler : IRequestHandler<ConfirmAssignmentUploadCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;

    public ConfirmAssignmentUploadCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(ConfirmAssignmentUploadCommand request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeInstructorAccessToAssignmentAsync(request.AssignmentId);
        if (!assignmentResult.IsSuccess) return Result.Failure(assignmentResult.Error!);

        var files = _unitOfWork.Assignments.GetFilesByAssignmentId(request.AssignmentId);

        foreach (var file in files)
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
