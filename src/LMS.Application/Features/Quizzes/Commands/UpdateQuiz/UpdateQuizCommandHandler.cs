using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly INotificationService _notificationService;

    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course), nameof(Quiz.Questions)]);

        if (quiz is null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var now = DateTime.UtcNow;

        var previousStatus = quiz.Status;

        var availableFromChanged = request.AvailableFrom != quiz.AvailableFrom;

        var quizStarted = quiz.AvailableFrom <= now;
        var quizEnded = quiz.AvailableUntil <= now;

        // -------------------------------------------------
        // General Rules
        // -------------------------------------------------

        if (availableFromChanged && request.AvailableFrom < now)
            return DomainErrors.Quiz.StartTimeCannotBeInThePast;

        // Draft -> Published
        if (previousStatus != QuizStatus.Published &&
            request.Status == QuizStatus.Published)
        {
            if (request.AvailableFrom <= now)
                return DomainErrors.Quiz.PublishedQuizCannotHaveStartTimeInThePast;

            if (!quiz.Questions.Any())
                return DomainErrors.Quiz.MustContainAtLeastOneQuestion;
        }

        if(previousStatus == QuizStatus.Published &&
           request.Status != QuizStatus.Published)
        {
            if (quizStarted)
                return DomainErrors.Common.Validation("QuizStatus", "Cannot change status of a started quiz.");
        }

        // -------------------------------------------------
        // Quiz Ended
        // -------------------------------------------------

        if (quizEnded)
        {
            if (request.AvailableUntil < quiz.AvailableUntil)
                return DomainErrors.Quiz.CannotShortenQuizDuration;

            if (request.MaximumAttempts < quiz.MaximumAttempts)
                return DomainErrors.Quiz.CannotDecreaseMaximumAttempts;

            if (request.AttemptTimeLimit < quiz.AttemptTimeLimit)
                return DomainErrors.Quiz.CannotDecreaseAttemptTimeLimit;

            quiz.Title = request.Title;
            quiz.Description = request.Description;
            quiz.ShowResultOnClose = request.ShowResultOnClose;
            quiz.ShuffleQuestions = request.ShuffleQuestions;
            quiz.ShuffleOptions = request.ShuffleOptions;

            quiz.MaximumAttempts = request.MaximumAttempts;
            quiz.AttemptTimeLimit = request.AttemptTimeLimit;

            quiz.AvailableUntil = request.AvailableUntil;
        }

        // -------------------------------------------------
        // Quiz Started (but not ended)
        // -------------------------------------------------

        else if (quizStarted)
        {
            if (request.MaximumAttempts < quiz.MaximumAttempts)
                return DomainErrors.Quiz.CannotDecreaseMaximumAttempts;

            if (request.AttemptTimeLimit < quiz.AttemptTimeLimit)
                return DomainErrors.Quiz.CannotDecreaseAttemptTimeLimit;

            if (request.AvailableUntil < quiz.AvailableUntil)
                return DomainErrors.Quiz.CannotShortenQuizDuration;

            quiz.Title = request.Title;
            quiz.Description = request.Description;
            quiz.ShowResultOnClose = request.ShowResultOnClose;
            quiz.ShuffleQuestions = request.ShuffleQuestions;
            quiz.ShuffleOptions = request.ShuffleOptions;

            quiz.MaximumAttempts = request.MaximumAttempts;
            quiz.AttemptTimeLimit = request.AttemptTimeLimit;
            quiz.AvailableUntil = request.AvailableUntil;
        }

        // -------------------------------------------------
        // Quiz Not Started
        // -------------------------------------------------

        else
        {
            quiz.Title = request.Title;
            quiz.Description = request.Description;

            quiz.AvailableFrom = request.AvailableFrom;
            quiz.AvailableUntil = request.AvailableUntil;

            quiz.MaximumAttempts = request.MaximumAttempts;
            quiz.AttemptTimeLimit = request.AttemptTimeLimit;

            quiz.ShowResultOnClose = request.ShowResultOnClose;
            quiz.ShuffleQuestions = request.ShuffleQuestions;
            quiz.ShuffleOptions = request.ShuffleOptions;
        }

        quiz.Status = request.Status;

        await _unitOfWork.CommitAsync(cancellationToken);

        if (previousStatus != QuizStatus.Published &&
            quiz.Status == QuizStatus.Published)
        {
            await _notificationService.NotifyAsync(
                quiz.CourseId,
                $"{quiz.Course.Name}: New Quiz",
                $"\"{quiz.Title}\" is now available. Start solving now!",
                NotificationType.NewQuizAdded,
                $"quizzes/{quiz.Id}",
                "Start Quiz");
        }

        return Result.Success("Quiz updated successfully");
    }
}
