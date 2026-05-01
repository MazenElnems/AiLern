using LMS.Application.Contracts.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.ProcessAIUpload;

public class ProcessAIUploadWebhookCommandHandler : IRequestHandler<ProcessAIUploadWebhookCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessAIUploadWebhookCommandHandler> _logger;

    public ProcessAIUploadWebhookCommandHandler(IUnitOfWork unitOfWork, ILogger<ProcessAIUploadWebhookCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ProcessAIUploadWebhookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing AI upload webhook for {ProjectId} file", request.Dto.ProjectId);

        var fileId = request.Dto.ProjectId;

        var aiResource = await _unitOfWork.AIResources.GetByIdAsync(fileId);

        if(aiResource != null && aiResource.AIStatus != request.Dto.Status)
        {
            aiResource.AIStatus = request.Dto.Status;
            _logger.LogInformation("Updated AI resource {FileId} status to {Status}", fileId, request.Dto.Status);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
