using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Courses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.ProcessAIUpload;

public class ProcessAIUploadWebhookCommandHandler : IRequestHandler<ProcessAIUploadWebhookCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessAIUploadWebhookCommandHandler> _logger;
    private readonly IAIServiceNotifier _aiStatusNotifier;

    public ProcessAIUploadWebhookCommandHandler(IUnitOfWork unitOfWork, ILogger<ProcessAIUploadWebhookCommandHandler> logger, IAIServiceNotifier aiStatusNotifier)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _aiStatusNotifier = aiStatusNotifier;
    }

    public async Task Handle(ProcessAIUploadWebhookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing AI upload webhook for {ProjectId} file", request.Dto.ProjectId);

        var fileId = request.Dto.ProjectId;

        var aiResource = await _unitOfWork.AIResources.GetAsync(a => a.Id == fileId,
            includeProperties: [nameof(AIResource.Course)]);

        if(aiResource != null && aiResource.AIStatus != request.Dto.Status)
        {
            aiResource.AIStatus = request.Dto.Status;
            _logger.LogInformation("Updated AI resource {FileId} status to {Status}", fileId, request.Dto.Status);
            await _unitOfWork.CommitAsync(cancellationToken);

            int instructorId = aiResource.Course.InstructorId;

            // send real-time status updates to the instructor
            await _aiStatusNotifier.NotifyStatusChangeAsync(aiResource.Id, instructorId.ToString(), aiResource.AIStatus, cancellationToken);
        }
    }
}
