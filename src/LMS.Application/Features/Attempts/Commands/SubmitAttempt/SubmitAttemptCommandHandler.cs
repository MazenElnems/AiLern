using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SubmitAttempt;

public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundJobService;

    public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var attempt = await _unitOfWork.Attempts.GetByIdAsync(request.AttemptId);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        attempt.Submit();
        await _unitOfWork.CommitAsync();

        _backgroundJobService.Delete(attempt.AutoSubmitJobId);

        return Result.Success("Submit Successfully");
    }
}
