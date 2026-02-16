using LMS.Application.CurrentUser;
using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Features.Assignments.Commands.DeleteAssignment;

public class AssignmentDeleteCommandHandler : IRequestHandler<AssignmentDeleteCommand, Result>
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

    public async Task<Result> Handle(AssignmentDeleteCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.Id,
            [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            return Result.Failure(DomainErrors.Assignment.NotFound(request.Id));

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to delete this assignment."));

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

        return Result.Success();
    }
}
