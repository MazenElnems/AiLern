using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.ProcessAIUpload;

public class ProcessAIUploadWebhookCommandHandler(IUnitOfWork unitOfWork, ILogger<ProcessAIUploadWebhookCommandHandler> logger, IAIServiceNotifier aiStatusNotifier, IWasabiService wsabiService) : IRequestHandler<ProcessAIUploadWebhookCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ProcessAIUploadWebhookCommandHandler> _logger = logger;
    private readonly IAIServiceNotifier _aiStatusNotifier = aiStatusNotifier;
    private readonly IWasabiService _wsabiService = wsabiService;

    public async Task Handle(ProcessAIUploadWebhookCommand request, CancellationToken cancellationToken)
    {
        var fileId = request.Dto.ProjectId;

        var aiResource = await _unitOfWork.AIResources.GetAsync(a => a.Id == fileId,
            includeProperties: [nameof(AIResource.Course)]);

        if(aiResource != null && aiResource.AIStatus != request.Dto.Status)
        {
            _logger.LogInformation("Updating AI resource {FileId} status to {Status}", fileId, request.Dto.Status);

            if(request.Dto.Status == AIStatus.Failed)
            {
                // delete file from Wasabi storage & database

                _logger.LogWarning("AI resource {FileId} failed to process. Error: {Error}", fileId, request.Dto.Error);

                if (aiResource.UploadStatus == UploadStatus.Completed)
                    await _wsabiService.DeleteFileAsync(aiResource.StoragePath, cancellationToken);

                _unitOfWork.AIResources.Delete(aiResource);
            }

            aiResource.AIStatus = request.Dto.Status;

            await _unitOfWork.CommitAsync(cancellationToken);
            int instructorId = aiResource.Course.InstructorId;
            // send real-time status updates to the instructor
            await _aiStatusNotifier.NotifyStatusChangeAsync(aiResource.Id, instructorId.ToString(), request.Dto.Status, request.Dto.Error, cancellationToken);
        }
    }
}
