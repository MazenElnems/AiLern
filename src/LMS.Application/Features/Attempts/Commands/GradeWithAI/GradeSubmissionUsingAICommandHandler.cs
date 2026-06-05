using LMS.Application.Common.Results;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeWithAI;

public class GradeSubmissionUsingAICommandHandler(IBackgroundJobService backgroundJobService, IUnitOfWork unitOfWork,
    IUserContext userContext, IGradingUsingAIJob gradingUsingAIJob)
    : IRequestHandler<GradeSubmissionUsingAICommand, Result>
{
    private readonly IBackgroundJobService _backgroundJobService = backgroundJobService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;
    private readonly IGradingUsingAIJob _gradingUsingAIJob = gradingUsingAIJob;

    public async Task<Result> Handle(GradeSubmissionUsingAICommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz is null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        _backgroundJobService.Enqueue(() => _gradingUsingAIJob.ExecuteAsync(request.QuizId, request.AttemptIds, cancellationToken));

        return Result.Success("Grading initiated with AI.");
    }
}
