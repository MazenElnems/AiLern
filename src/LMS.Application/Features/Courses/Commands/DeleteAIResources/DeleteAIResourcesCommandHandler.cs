using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.DeleteAIResources;

public class DeleteAIResourcesCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IWasabiService wasabi,
    IAIService aiService,
    ILogger<DeleteAIResourcesCommandHandler> logger) : IRequestHandler<DeleteAIResourcesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _user = userContext;
    private readonly IWasabiService _wasabi = wasabi;
    private readonly IAIService _aiService = aiService;
    private readonly ILogger<DeleteAIResourcesCommandHandler> _logger = logger;

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

        if(aiResource.AIStatus == AIStatus.Processing)
        {
            _logger.LogWarning(
                "Cannot delete file {ProjectId} from AI Service because it is still processing",
                aiResource.Id
            );
            return DomainErrors.AiResource.CannotDeleteProcessingFile;
        }

        if (aiResource.AIStatus == AIStatus.Completed)
        {
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
        }

        _unitOfWork.AIResources.Delete(aiResource);

        if (await _wasabi.FileExists(aiResource.StoragePath))
            await _wasabi.DeleteFileAsync(aiResource.StoragePath, cancellationToken, true);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Deleted Success");
    }
}
