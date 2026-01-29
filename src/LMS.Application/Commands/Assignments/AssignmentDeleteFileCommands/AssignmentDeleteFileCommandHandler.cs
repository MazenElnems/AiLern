using LMS.Application.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Commands.Assignments.AssignmentDeleteFileCommands;

public class AssignmentDeleteFileCommandHandler : IRequestHandler<AssignmentDeleteFileCommand>
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

    public async Task Handle(AssignmentDeleteFileCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.AssignmentId,
            [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.AssignmentId.ToString());

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to delete this assignment file.");

        var file = assignment.Files.FirstOrDefault(f => f.Id == request.FileId);
        if (file == null)
            throw new ResourceNotFoundException(nameof(AssignmentFile), request.FileId.ToString());

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
    }
}
