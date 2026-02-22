using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
namespace LMS.Application.Features.Assignments.Commands.ConfirmFileUpload;

public class ConfirmAssignmentUploadCommandHandler : IRequestHandler<ConfirmAssignmentUploadCommand, Result>
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

    public async Task<Result> Handle(ConfirmAssignmentUploadCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var assignment = await _unitOfWork.Assignments
            .GetAsync(a => a.Id == request.AssignmentId, [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            return Result.Failure(DomainErrors.Assignment.NotFound(request.AssignmentId));

        var course = assignment.Course;

        if (course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to request pre-signed URLs for this assignment."));

        foreach (var file in assignment.Files)
        {
            var exists = await _wasabiService.FileExists(file.StoragePath);

            if(!exists)
                return Result.Failure(DomainErrors.Storage.FileMissing);

            file.UploadStatus = UploadStatus.Completed;
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
