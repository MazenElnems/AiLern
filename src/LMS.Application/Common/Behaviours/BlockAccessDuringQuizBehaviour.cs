using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Common.Behaviours;

public class BlockAccessDuringQuizBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICourseContentRequest<TResponse>
    where TResponse : IResult 
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BlockAccessDuringQuizBehaviour<TRequest, TResponse>> _logger;

    public BlockAccessDuringQuizBehaviour(IUnitOfWork unitOfWork, IUserContext userContext, ILogger<BlockAccessDuringQuizBehaviour<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Accessing course content: check if the current student has in-progress attempt");
        var currentUser = _userContext.GetCurrentUser();

        if (currentUser.IsInRole(UserRoles.Student))
        {
            var hasInProgressAttempt = await _unitOfWork.Attempts.HasInProgressAttemptAsync(request.CourseId, currentUser.Id);

            if (hasInProgressAttempt)
            {
                _logger.LogWarning("Student {Student} try to access course content during the quiz", currentUser);
                return (TResponse)TResponse.Failure(DomainErrors.Common.Forbidden("Cannot access course content during In-Progress Attempt."));
            }
        }

        return await next(cancellationToken);
    }
}
