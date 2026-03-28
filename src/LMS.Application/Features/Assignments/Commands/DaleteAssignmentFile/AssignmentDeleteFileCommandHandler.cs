using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.DaleteAssignmentFile;

public class AssignmentDeleteFileCommandHandler : IRequestHandler<AssignmentDeleteFileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;

    public AssignmentDeleteFileCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(AssignmentDeleteFileCommand request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeInstructorAccessToAssignmentAsync(request.AssignmentId);
        if (!assignmentResult.IsSuccess) return Result.Failure(assignmentResult.Error!);

        var files = _unitOfWork.Assignments.GetFilesByAssignmentId(request.AssignmentId);
        var file = files.FirstOrDefault(f => f.Id == request.FileId);
        if (file == null)
            return Result.Failure(DomainErrors.AssignmentFile.NotFound(request.FileId));

        var filePath = file.StoragePath;
        try
        {
            await _wasabiService.DeleteFileAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete file from storage.", ex);
        }

        _unitOfWork.Assignments.DeleteFile(file);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
