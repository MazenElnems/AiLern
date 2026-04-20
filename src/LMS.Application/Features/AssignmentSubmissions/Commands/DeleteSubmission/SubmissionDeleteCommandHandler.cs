using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Application.Features.Assignments.Commands.DeleteAssignment;
using LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission;
using LMS.Domain.Errors;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Commands.Submissions.SubmissionDeleteCommands;

public class SubmissionDeleteCommandHandler : IRequestHandler<SubmissionDeleteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<AssignmentDeleteCommandHandler> _logger;

    public SubmissionDeleteCommandHandler(ILogger<AssignmentDeleteCommandHandler> logger, IWasabiService wasabiService, IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _wasabiService = wasabiService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmissionDeleteCommand request, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.AssignmentSubmissions.GetAsync(a=>a.Id == request.Id,
            includeProperties: [nameof(AssignmentSubmission.Files)]);
        if (submission is null)
            return Result.Failure(DomainErrors.AssignmentSubmission.NotFound(request.Id.ToString()));
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(submission.AssignmentId);
        if (assignment is null)
            return Result.Failure(DomainErrors.Assignment.NotFound(submission.AssignmentId));
        var user = _userContext.GetCurrentUser();
        if (submission.StudentId != user.Id || submission.Feedback != null)
        {
            return Result.Failure(DomainErrors.AssignmentSubmission.DeleteForbidden);
        }
        if (assignment.AllowLateSubmission == false && assignment.DueDate <= DateTime.UtcNow)
        {
            return Result.Failure(DomainErrors.AssignmentSubmission.DeleteAfterDeadline);
        }
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
