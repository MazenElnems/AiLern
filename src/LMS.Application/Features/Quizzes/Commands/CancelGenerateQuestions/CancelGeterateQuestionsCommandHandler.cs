using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CancelGenerateQuestions;

public class CancelGeterateQuestionsCommandHandler : IRequestHandler<CancelGeterateQuestionsCommand, Result>
{
    private readonly IPermissionService _permissionService;
    private readonly IBackgroundJobService _backgroundService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelGeterateQuestionsCommandHandler(IPermissionService permissionService, IBackgroundJobService backgroundService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _backgroundService = backgroundService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelGeterateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var job = await _unitOfWork.QuestionGenerationJobs.GetAsync(x => x.Id == request.id,
            includeProperties: [nameof(Quiz)]);

        if (job == null)
            return DomainErrors.QuestionGenerationJobs.NotFound(request.id);

        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(job.Quiz.CourseId);
        if (!courseResult.IsSuccess) return courseResult.Error!;

        if (job.Status != AIJobStatus.InProgress)
            return DomainErrors.QuestionGenerationJobs.NotInProgress;

        _backgroundService.Delete(job.HangfireJobId);

        return Result.Success("Job canceled Successfuly.");
    }
}
