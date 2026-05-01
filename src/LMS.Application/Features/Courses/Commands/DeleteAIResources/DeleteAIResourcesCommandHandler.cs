using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.DeleteAIResources;

public class DeleteAIResourcesCommandHandler : IRequestHandler<DeleteAIResourcesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly IWasabiService _wasabi;
    private readonly IAIService _aiService;
    private readonly ILogger<DeleteAIResourcesCommandHandler> _logger;

    public DeleteAIResourcesCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IWasabiService wasabi,
        IAIService aiService,
        ILogger<DeleteAIResourcesCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _user = userContext;
        _wasabi = wasabi;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteAIResourcesCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        var aiResource = await _unitOfWork.AIResources.GetByIdAsync(request.AiResourceId);

        if (aiResource == null || aiResource.CourseId != request.CourseId)
            return DomainErrors.AiResource.NotFound(request.AiResourceId);

        _logger.LogInformation("Deleting File {ProjectId} from AI Service", aiResource.Id);

        var result = await _aiService.DeleteFileAsync(aiResource.Id.ToString(), cancellationToken);

        if(result.Status != "ok")
        {
            _logger.LogWarning(
                "Failed to delete file {ProjectId} from AI Service: {Message}",
                aiResource.Id,
                result.Message
            );

            return DomainErrors.AiResource.ErrorWhileDeletingFile;
        }

        _logger.LogInformation(
            "File {ProjectId} {ChunksDeleted} deleted successfully from AI Service : {Message}",
            aiResource.Id,
            result.CunksDeleted,
            result.Message
        );

        _unitOfWork.AIResources.Delete(aiResource);
        var isExist = await _wasabi.FileExists(aiResource.StoragePath);

        if (!isExist)
            return DomainErrors.Common.BusinessRule("FileNotFound","The file does not exist in storage.");

        await _wasabi.DeleteFileAsync(aiResource.StoragePath, cancellationToken, true);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Deleted Success");
    }
}
