using LMS.Core.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Commands.Assignments.ConfirmAssignmentUploadCommands;

public class ConfirmAssignmentUploadCommandHandler : IRequestHandler<ConfirmAssignmentUploadCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;

    public ConfirmAssignmentUploadCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
    }

    public async Task Handle(ConfirmAssignmentUploadCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var assignment = await _unitOfWork.Assignments
            .GetAsync(a => a.Id == request.AssignmentId, [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.AssignmentId.ToString());

        var course = assignment.Course;

        if (course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to request pre-signed URLs for this assignment.");

        foreach (var file in assignment.Files)
        {
            var exists = await _wasabiService.FileExists(file.StoragePath);

            if(!exists)
                throw new ValidationException("File does not exist in storage.");

            file.UploadStatus = UploadStatus.Completed;
        }

        await _unitOfWork.CommitAsync();
    }
}
