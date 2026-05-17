using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Jobs;

public class PrepareDocumentsForAIJob(
    IUnitOfWork unitOfWork,
    IWasabiService wasabiService,
    IAIService aIService,
    IAIServiceNotifier aiServiceNotifier,
    ILogger<PrepareDocumentsForAIJob> logger) : IPrepareDocumentsForAIJob
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWasabiService _wasabiService = wasabiService;
    private readonly IAIService _aIService = aIService;
    private readonly IAIServiceNotifier _aiServiceNotifier = aiServiceNotifier;
    private readonly ILogger<PrepareDocumentsForAIJob> _logger = logger;

    public async Task ExecuteAsync(int courseId, int instructorId, CancellationToken cancellationToken)
    {
        var docResources = await _unitOfWork.AIResources.Query
            .Where(f => f.CourseId == courseId && f.UploadStatus == UploadStatus.Completed && f.AIStatus == AIStatus.Pending)
            .ToListAsync(cancellationToken);

        var aIUploadDocsRequest = new List<AIUploadDocsRequest>();

        foreach (var doc in docResources)
        {
            var url = await _wasabiService.GeneratePresignedDownloadUrlAsync(doc.StoragePath, 30);
            aIUploadDocsRequest.Add(new AIUploadDocsRequest { PresignedUrl = url, ProjectId = doc.Id.ToString() });
        }

        var result = new AIUploadDocsResponse();
        try
        {
            result = await _aIService.UploadDocsAsContextAsync(aIUploadDocsRequest, cancellationToken);
        }
        catch (AIServiceTimeoutException ex)
        {
            UpdateAIStatus(AIStatus.Failed, docResources);

            await _aiServiceNotifier.NotifyAIServiceProblemAsync(
                instructorId.ToString(),
                "AI service timeout occurred while uploading documents.",
                cancellationToken
            );

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            UpdateAIStatus(AIStatus.Failed, docResources);

            await _aiServiceNotifier.NotifyAIServiceProblemAsync(
                instructorId.ToString(),
                "Can't connect to AI service right now.",
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            UpdateAIStatus(AIStatus.Failed, docResources);

            await _aiServiceNotifier.NotifyAIServiceProblemAsync(
                instructorId.ToString(),
                "An error occurred while uploading documents to AI service.",
                cancellationToken
            );
        }
        finally
        {
            _logger.LogInformation("AI Upload Documents Result: {Status}, Message: {Message}",
                result.Status, result.Message);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
    private void UpdateAIStatus(AIStatus status, List<AIResource> aIResources)
    {
        foreach (var resource in aIResources)
        {
            resource.AIStatus = status;
        }
    }
}
