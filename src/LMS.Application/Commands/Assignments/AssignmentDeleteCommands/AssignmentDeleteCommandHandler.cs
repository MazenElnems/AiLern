using LMS.Application.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Assignments.AssignmentDeleteCommands;

public class AssignmentDeleteCommandHandler : IRequestHandler<AssignmentDeleteCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IWasabiService _wasabiService;
    private readonly ILogger<AssignmentDeleteCommandHandler> _logger;

    public AssignmentDeleteCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IWasabiService wasabiService, ILogger<AssignmentDeleteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _wasabiService = wasabiService;
        _logger = logger;
    }

    public async Task Handle(AssignmentDeleteCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.Id,
            [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.Id.ToString());

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to delete this assignment.");

        var filePaths = assignment.Files.Select(f => f.StoragePath);

        _unitOfWork.Assignments.Delete(assignment);
        await _unitOfWork.CommitAsync();

        try
        {
            foreach (var filePath in filePaths)
            {
                await _wasabiService.DeleteFileAsync(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting assignment files from Wasabi.");
        }
    }
}
