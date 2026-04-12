using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Attempts.Commands.CreateAttempt;

public class CreateAttemptCommandHandler : IRequestHandler<CreateAttemptCommand, Result<AttemptDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IAutoSubmitAttemptJob _autoSubmitAttemptJob;
    private readonly ILogger<CreateAttemptCommandHandler> _logger;

    public CreateAttemptCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, ILogger<CreateAttemptCommandHandler> logger, IBackgroundJobService backgroundJobService, IAutoSubmitAttemptJob autoSubmitAttemptJob)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _backgroundJobService = backgroundJobService;
        _autoSubmitAttemptJob = autoSubmitAttemptJob;
    }

    public async Task<Result<AttemptDto>> Handle(CreateAttemptCommand request, CancellationToken cancellationToken)
    {
        var jobId = string.Empty;
        try
        {
            var user = _userContext.GetCurrentUser();

            var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
                includeProperties: [nameof(Quiz.Course)]);

            if (quiz == null)
                return DomainErrors.Quiz.NotFound(request.QuizId);

            if (!await _unitOfWork.Enrollments.IsEnrolledAsync(quiz.CourseId, user.Id))
                return DomainErrors.Course.NotEnrolled;

            // quiz is not published
            if (quiz.Status != QuizStatus.Published)
                return DomainErrors.Quiz.NotPublished;

            var now = DateTime.UtcNow;

            // quiz is expired or not yet available
            if (quiz.AvailableFrom > now || quiz.AvailableUntil < now)
                return DomainErrors.Quiz.QuizNotAvailableAtThisTime;

            var studentAttemptsCount = await _unitOfWork.Attempts.CountAsync(
                a => a.StudentId == user.Id &&
                a.QuizId == quiz.Id);

            // student has reached the maximum attempts for this quiz
            if (studentAttemptsCount >= quiz.MaximumAttempts)
                return DomainErrors.Attempt.MaximumAttemptsReaches;

            var hasInProgressAttempt = await _unitOfWork.Attempts.AnyAsync(
                a => a.StudentId == user.Id &&
                     a.QuizId == request.QuizId &&
                     a.Status == AttemptStatus.InProgress);

            // student has no current in-progress attempt for this quiz
            if (hasInProgressAttempt)
                return DomainErrors.Attempt.AnotherAttemptSessionStarted;

            var quizQuestionIds = await _unitOfWork.Questions.GetQuestionIdsByQuizIdAsync(quiz.Id);

            var attempt = Attempt.StartNew(
                studentId: user.Id,
                attemptNumber: studentAttemptsCount + 1,
                startAt: now,
                attemptEndTime: quiz.CalculateAttemptEndTime(now),
                questionIds: quizQuestionIds);

            jobId = _backgroundJobService.Schedule(
                () => _autoSubmitAttemptJob.ExecuteAsync(attempt.Id, cancellationToken),
                attempt.AttemptEndTime.AddSeconds(5) - now);

            attempt.AutoSubmitJobId = jobId;

            quiz.Attempts.Add(attempt);
            await _unitOfWork.CommitAsync();

            return new AttemptDto { AttemptId = attempt.Id, AttemptEndDate = attempt.AttemptEndTime };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Cannot Add new Attempt for {@Student} due to Concurrency Conflict.",
                _userContext.GetCurrentUser());

            _backgroundJobService.Delete(jobId);
            return DomainErrors.Attempt.DuplicateAttempt;
        }
    }
}
