using LMS.Application.Common.Results;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SubmitAttempt;

public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly ICalculateStudentScoreJob _calculateStudentScoreJob;

    public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBackgroundJobService backgroundJobService, ICalculateStudentScoreJob calculateStudentScoreJob)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _backgroundJobService = backgroundJobService;
        _calculateStudentScoreJob = calculateStudentScoreJob;
    }

    public async Task<Result> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var attempt = await _unitOfWork.Attempts.GetByIdAsync(request.AttemptId);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != user.Id)
            return DomainErrors.Attempt.NotOwned;

        attempt.Submit();
        await _unitOfWork.CommitAsync(cancellationToken);

        // Delete the auto submit background job
        _backgroundJobService.Delete(attempt.AutoSubmitJobId);

        _backgroundJobService.Enqueue(() => _calculateStudentScoreJob.ExecuteAsync(attempt.Id, cancellationToken));

        return Result.Success("Submit Successfully");
    }
}
