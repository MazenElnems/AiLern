using LMS.Application.Common.Results;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Attempts.Commands.GradeAttemptByAI;

public class GradeByAICommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IBackgroundJobService backgroundJobService,
    IAIBatchGradingJob aiBatchGradingJob
    ) : IRequestHandler<GradeByAICommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;
    private readonly IBackgroundJobService _backgroundJobService = backgroundJobService;
    private readonly IAIBatchGradingJob _aiBatchGradingJob = aiBatchGradingJob;

    public async Task<Result> Handle(GradeByAICommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var attemptsToGrade = await _unitOfWork.Attempts.Query
            .Where(a => request.AtteptIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        // Idempotency check
        if (attemptsToGrade.Any(a => a.AIGradingStatus == AIGradingStatus.InProgress))
            return Error.Conflict("AttemptGradingInProgress", "an attempt currently graded by ai.");

        if (attemptsToGrade.Any(a => a.Status == AttemptStatus.InProgress))
            return Error.BusinessRule("AttemptInProgress", "can't grade InProgress Attempt.");

        _backgroundJobService.Enqueue(
            () => _aiBatchGradingJob.ExecuteAsync(quiz.CourseId, request.QuizId, request.AtteptIds, cancellationToken)
        );

        foreach (var attempt in attemptsToGrade)
        {
            attempt.AIGradingStatus = AIGradingStatus.InProgress;
            attempt.IsAIGraded = true;
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("AI Grading start processing.");
    }
}
