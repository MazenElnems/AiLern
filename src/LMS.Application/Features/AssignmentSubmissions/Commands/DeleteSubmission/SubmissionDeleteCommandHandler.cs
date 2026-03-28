using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Submissions.SubmissionDeleteCommands;

public class SubmissionDeleteCommandHandler : IRequestHandler<LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission.SubmissionDeleteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<SubmissionDeleteCommandHandler> _logger;

    public SubmissionDeleteCommandHandler(ILogger<SubmissionDeleteCommandHandler> logger, IWasabiService wasabiService, IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _wasabiService = wasabiService;
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission.SubmissionDeleteCommand request, CancellationToken cancellationToken)
    {
        var submissionResult = await _permissionService.AuthorizeStudentAccessToSubmissionAsync(request.Id);
        if (!submissionResult.IsSuccess) return Result.Failure(submissionResult.Error!);
        var submission = submissionResult.Value!;

        var assignment = await _unitOfWork.Assignments.GetByIdAsync(submission.AssignmentId);
        if (assignment is null)
            return Result.Failure(DomainErrors.Assignment.NotFound(submission.AssignmentId));

        if (assignment.AllowLateSubmission == false)
            return Result.Failure(DomainErrors.AssignmentSubmission.DeleteAfterDeadline);

        var filePaths = submission.Files.Select(f => f.StoragePath);

        _unitOfWork.AssignmentSubmissions.Delete(submission);
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
            _logger.LogError(ex, "An error occurred while deleting submission files from Wasabi.");
        }
        return Result.Success();
    }
}
