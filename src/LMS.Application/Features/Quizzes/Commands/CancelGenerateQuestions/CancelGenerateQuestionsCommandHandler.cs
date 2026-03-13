using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CancelGenerateQuestions;

public class CancelGenerateQuestionsCommandHandler : IRequestHandler<CancelGeterateQuestionsCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IBackgroundJobService _backgroundService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelGenerateQuestionsCommandHandler(IUserContext userContext, IBackgroundJobService backgroundService, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _backgroundService = backgroundService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelGeterateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var job = await _unitOfWork.QuestionGenerationJobs.GetAsync(x => x.Id == request.id, includeProperties: [nameof(Quiz)]);

        if (job == null)
            return DomainErrors.QuestionGenerationJobs.NotFound(request.id);

        var course = await _unitOfWork.Courses.GetByIdAsync(job.Quiz.CourseId);
        if (course == null)
            return DomainErrors.Course.NotFound(job.Quiz.CourseId);

        if (course.InstructorId != userId)
            return DomainErrors.Quiz.NotOwned;

        if (job.Status != Domain.Enums.AIJobStatus.InProgress)
            return DomainErrors.QuestionGenerationJobs.NotInProgress;

        _backgroundService.Delete(job.HangfireJobId);

        return Result.Success("Job canceled Successfuly.");





    }
}
