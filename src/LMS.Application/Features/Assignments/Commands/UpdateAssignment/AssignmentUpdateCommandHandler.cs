using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommandHandler : IRequestHandler<AssignmentUpdateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AssignmentUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(AssignmentUpdateCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.Id,
            [nameof(Assignment.Course)]);

        if (assignment == null)
            return Result.Failure(DomainErrors.Assignment.NotFound(request.Id));

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to update this assignment."));

        assignment.Title = request.Title;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.AllowLateSubmission = request.AllowLateSubmission;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
