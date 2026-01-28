using LMS.Core.Commands.Assignments.AssignmentDeleteCommands;
using LMS.Core.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Submissions.SubmissionDeleteCommands;

public class SubmissionDeleteCommandHandler : IRequestHandler<SubmissionDeleteCommand>
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

    public async Task Handle(SubmissionDeleteCommand request, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(request.Id);
        if (submission is null)
            throw new ResourceNotFoundException(nameof(submission), request.Id.ToString());
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(submission.AssignmentId);
        if (assignment is null)
            throw new ResourceNotFoundException(nameof(assignment), submission.AssignmentId.ToString());
        var user = _userContext.GetCurrentUser();
        if (submission.StudentId != user.Id)
        {
            throw new ForbiddenException("You cannot delete another student's submission.");
        }
        if (assignment.AllowLateSubmission == false)
        {
            throw new ForbiddenException("Submission deletion is not allowed after the assignment deadline.");
        }
        var filePaths = submission.Files.Select(f => f.StoragePath);

        _unitOfWork.Submissions.Delete(submission);
        await _unitOfWork.CommitAsync();

        try
        {
            foreach (var filePath in filePaths)
            {
                await _wasabiService.DeleteFileAsync(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting submission files from Wasabi.");
        }
    }
}
