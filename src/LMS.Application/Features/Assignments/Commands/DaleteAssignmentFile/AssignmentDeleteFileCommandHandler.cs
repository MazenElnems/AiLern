using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Errors;

namespace LMS.Application.Features.Assignments.Commands.DaleteAssignmentFile;

public class AssignmentDeleteFileCommandHandler : IRequestHandler<AssignmentDeleteFileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;

    public AssignmentDeleteFileCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
    }

    public async Task<Result> Handle(AssignmentDeleteFileCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.AssignmentId,
            [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            return Result.Failure(DomainErrors.Assignment.NotFound(request.AssignmentId));

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to delete this assignment file."));

        var file = assignment.Files.FirstOrDefault(f => f.Id == request.FileId);
        if (file == null)
            return Result.Failure(DomainErrors.AssignmentFile.NotFound(request.FileId));

        var filePath = file.StoragePath;
        try
        {
            await _wasabiService.DeleteFileAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to delete file from storage.", ex);
        }

        _unitOfWork.Assignments.DeleteFile(file);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
