using LMS.Application.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentPublishCommands;

public class AssignmentPublishCommandHandler : IRequestHandler<AssignmentPublishCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AssignmentPublishCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task Handle(AssignmentPublishCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.AssignmentId, [nameof(Assignment.Course)]);

        if (assignment == null)
            throw new ResourceNotFoundException("Assignment", request.AssignmentId.ToString());

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to publish this assignment.");

        assignment.IsPublished = true;
        await _unitOfWork.CommitAsync();
    }
}
