using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.ConfirmAIResources;

public class ConfirmAIResourcesCommandHandler : IRequestHandler<ConfirmAIResourcesCommand, Result<List<string>>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IPrepareDocumentsForAIJob _prepareDocumentsForAIJob;


    public ConfirmAIResourcesCommandHandler(IUserContext user, IUnitOfWork unitOfWork, IBackgroundJobService backgroundJobService, IPrepareDocumentsForAIJob prepareDocumentsForAIJob)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _backgroundJobService = backgroundJobService;
        _prepareDocumentsForAIJob = prepareDocumentsForAIJob;
    }

    public async Task<Result<List<string>>> Handle(ConfirmAIResourcesCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.AIResources)]);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        var unSuccessfulAIResources = new List<string>();
        var aiResources = course.AIResources.Where(ar => ar.UploadStatus != UploadStatus.Completed).ToList();

        foreach (var aiResource in aiResources)
        {
            if (request.AiResourceIds.Contains(aiResource.Id))
            {
                aiResource.UploadStatus = UploadStatus.Completed;
            }
            else
            {
                course.AIResources.Remove(aiResource);
                unSuccessfulAIResources.Add(aiResource.FileName);
            }
        }
        await _unitOfWork.CommitAsync(cancellationToken);

        //_backgroundJobService.Enqueue(() => _prepareDocumentsForAIJob.ExecuteAsync(request.CourseId, userId, cancellationToken));
        return unSuccessfulAIResources;
    }
}
