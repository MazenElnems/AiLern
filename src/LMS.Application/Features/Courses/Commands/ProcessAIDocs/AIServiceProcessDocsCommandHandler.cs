using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands.ProcessAIDocs;

public class AIServiceProcessDocsCommandHandler(
    IUnitOfWork unitOfWork,
    IAIService aiService,
    ILogger<AIServiceProcessDocsCommandHandler> logger,
    IUserContext userContext,
    IWasabiService wasabiService) : IRequestHandler<AIServiceProcessDocsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;
    private readonly IWasabiService _wasabiService = wasabiService;
    private readonly IAIService _aiService = aiService;
    private readonly ILogger<AIServiceProcessDocsCommandHandler> _logger = logger;

    public async Task<Result> Handle(AIServiceProcessDocsCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != instructorId)
            return DomainErrors.Course.NotOwned;

        // process only the documents that are uploaded and not currently being processed by the AI service.
        // so it allow ai status `Failed` for re-try and `Pending` for the documents that are not processed yet.
        var aiResources = await _unitOfWork.AIResources.Query
            .Where(r => request.DocumentIds.Contains(r.Id) &&
                              r.UploadStatus == UploadStatus.Completed &&
                              r.AIStatus != AIStatus.Processing &&
                              r.AIStatus != AIStatus.Completed
            ).ToListAsync(cancellationToken);

        var aiUploadRequest = new List<AIUploadDocsRequest>();

        foreach (var aiResource in aiResources)
        {
            var presignedUrl = await _wasabiService.GeneratePresignedDownloadUrlAsync(aiResource.StoragePath);
            aiUploadRequest.Add(new AIUploadDocsRequest
            {
                PresignedUrl = presignedUrl,
                ProjectId = aiResource.Id.ToString()
            });
        }

        try
        {
            await _aiService.UploadDocsAsContextAsync(aiUploadRequest, cancellationToken);
        }
        catch(Exception ex)
        {
            await UpdateAIResourceStatus(aiResources, AIStatus.Failed);
            throw;
        }

        return Result.Success("Documents are being processed by the AI service.");
    }

    private async Task UpdateAIResourceStatus(List<AIResource> aiResources, AIStatus status)
    {
        foreach (var aiResource in aiResources)
        {
            aiResource.AIStatus = status;
        }
        await _unitOfWork.CommitAsync();
    }
}
