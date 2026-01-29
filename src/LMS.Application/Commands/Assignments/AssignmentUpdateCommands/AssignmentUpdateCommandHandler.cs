using LMS.Application.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentUpdateCommands;

public class AssignmentUpdateCommandHandler : IRequestHandler<AssignmentUpdateCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AssignmentUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task Handle(AssignmentUpdateCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.Id,
            [nameof(Assignment.Course)]);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.Id.ToString());

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to update this assignment.");

        assignment.Title = request.Title;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.AllowLateSubmission = request.AllowLateSubmission;

        await _unitOfWork.CommitAsync();
    }
}
