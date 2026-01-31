using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.PublishAssignment;

public class AssignmentPublishCommandHandler : IRequestHandler<AssignmentPublishCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AssignmentPublishCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(AssignmentPublishCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.AssignmentId, [nameof(Assignment.Course)]);

        if (assignment == null)
            return Result.Failure(DomainErrors.Assignment.NotFound(request.AssignmentId));

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to publish this assignment."));

        assignment.IsPublished = true;
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
