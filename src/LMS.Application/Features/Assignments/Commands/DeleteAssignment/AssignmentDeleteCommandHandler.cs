using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Assignments.Commands.DeleteAssignment;

public class AssignmentDeleteCommandHandler : IRequestHandler<AssignmentDeleteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<AssignmentDeleteCommandHandler> _logger;

    public AssignmentDeleteCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IWasabiService wasabiService, ILogger<AssignmentDeleteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignmentDeleteCommand request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeInstructorAccessToAssignmentAsync(request.Id);
        if (!assignmentResult.IsSuccess) return Result.Failure(assignmentResult.Error!);
        var assignment = assignmentResult.Value!;

        var filePaths = _unitOfWork.Assignments.GetFilesByAssignmentId(request.Id)
            .Select(f => f.StoragePath);

        _unitOfWork.Assignments.Delete(assignment);
        await _unitOfWork.CommitAsync();

        try
        {
            foreach (var filePath in filePaths)
            {
                await _wasabiService.DeleteFileAsync(filePath, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting assignment files from Wasabi.");
        }

        return Result.Success();
    }
}
