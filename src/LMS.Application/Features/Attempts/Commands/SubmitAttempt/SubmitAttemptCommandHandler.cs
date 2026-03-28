using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SubmitAttempt;

public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IBackgroundJobService _backgroundJobService;

    public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
    {
        var attemptResult = await _permissionService.AuthorizeStudentAccessToAttemptAsync(request.AttemptId);
        if (!attemptResult.IsSuccess) return Result.Failure(attemptResult.Error!);
        var attempt = attemptResult.Value!;

        attempt.Submit();
        await _unitOfWork.CommitAsync();

        _backgroundJobService.Delete(attempt.AutoSubmitJobId);

        return Result.Success("Submit Successfully");
    }
}
