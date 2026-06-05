using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly INotificationService _notificationService;

    public GradeSubmissionCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Attempt.Answers)]);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.Id);

        var previousStatus = attempt.Status;
        var previousAIGradingStatus = attempt.AIGradingStatus;

        var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == attempt.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(attempt.QuizId);

        if (quiz.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        if (attempt.Status == AttemptStatus.InProgress)
            return DomainErrors.Attempt.StillInProgress;

        var answersDictionary = attempt.Answers
            .ToDictionary(a => a.QuestionId);

        foreach (var gradeDto in request.Grades)
        {
            if (!answersDictionary.TryGetValue(gradeDto.QuestionId, out var attemptAnswer))
                return DomainErrors.Common.NotFound("Question", $"{gradeDto.QuestionId}");

            attemptAnswer.Mark = gradeDto.Score;
            attemptAnswer.Feedback = gradeDto.Feedback;
        }

        attempt.Status = request.Status;

        if(previousStatus != AttemptStatus.Graded && attempt.Status == AttemptStatus.Graded)
        {
            await _notificationService.NotifyUserWithEmailAsync(
                attempt.StudentId,
                $"{quiz.Course.Name}: Quiz Graded",
                $"Your quiz \"{quiz.Title}\" has been graded. Check your result now.",
                NotificationType.AttemptReviewed,
                $"https://www.ailern.me/quizzes/{quiz.Id}/result",
                "View Result"
            );
        }

        if (previousAIGradingStatus == AIGradingStatus.Graded)
            attempt.AIGradingStatus = AIGradingStatus.Overwritten;

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success("Attempt graded successfully.");
    }
}
