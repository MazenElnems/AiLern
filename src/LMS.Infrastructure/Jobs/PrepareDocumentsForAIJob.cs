using LMS.Application.Common.Models.Request;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Jobs;

public class PrepareDocumentsForAIJob : IPrepareDocumentsForAIJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWasabiService _wasabiService;
    private readonly IAIService _aIService;
    public PrepareDocumentsForAIJob(IUnitOfWork unitOfWork, IWasabiService wasabiService, IAIService aIService)
    {
        _unitOfWork = unitOfWork;
        _wasabiService = wasabiService;
        _aIService = aIService;
    }

    public async Task ExecuteAsync(int courseId, CancellationToken cancellationToken)
    {
        var docResources = await _unitOfWork.AIResources.Query
            .Where(f => f.CourseId == courseId)
            .ToListAsync(cancellationToken);

        var aIUploadDocsRequest = new List<AIUploadDocsRequest>();

        foreach(var doc in docResources)
        {
            var url = await _wasabiService.GeneratePresignedDownloadUrlAsync(doc.StoragePath, 30);
            aIUploadDocsRequest.Add(new AIUploadDocsRequest { PresignedUrl = url, ProjectId = doc.Id.ToString() });
        }

        var result = await _aIService.UploadFilesAsContextAsync(aIUploadDocsRequest, cancellationToken);

        if(result.Status != "accepted")
        {
            foreach(var d in docResources)
            {
                d.AIStatus = AIStatus.Failed;
            }
            await _unitOfWork.CommitAsync(cancellationToken);
            return;
        }
    }
}
