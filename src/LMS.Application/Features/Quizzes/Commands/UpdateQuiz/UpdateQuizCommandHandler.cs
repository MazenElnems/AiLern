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

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if(quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if(quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var now = DateTime.UtcNow;

        var previousStatus = quiz.Status;

        // Quiz started And Published
        if (quiz.AvailableFrom < now && quiz.Status == QuizStatus.Published)
        {
            // Can only increase AvailableUntil, MaximumAttempts, AttemptTimeLimit
            if (request.MaximumAttempts < quiz.MaximumAttempts)
                return DomainErrors.Quiz.CannotDecreaseMaximumAttempts;

            if (request.AttemptTimeLimit < quiz.AttemptTimeLimit)
                return DomainErrors.Quiz.CannotDecreaseAttemptTimeLimit;

            if (request.AvailableUntil < quiz.AvailableUntil)
                return DomainErrors.Quiz.CannotShortenQuizDuration;
        }
        // Draft Quiz or Quiz not started
        else
        {
            // Try to set start time in the past
            if (request.AvailableFrom < now)
                return DomainErrors.Quiz.StartTimeCannotBeInThePast;

            quiz.Title = request.Title;
            quiz.Description = request.Description;
            quiz.AvailableFrom = request.AvailableFrom;
        }

        quiz.MaximumAttempts = request.MaximumAttempts;
        quiz.AvailableUntil = request.AvailableUntil;
        quiz.AttemptTimeLimit = request.AttemptTimeLimit;
        quiz.ShowResultOnClose = request.ShowResultOnClose;
        quiz.ShuffleQuestions = request.ShuffleQuestions;
        quiz.ShuffleOptions = request.ShuffleOptions;

        await _unitOfWork.CommitAsync(cancellationToken);

        if (previousStatus != QuizStatus.Published && quiz.Status == QuizStatus.Published)
        {
            await _notificationService.NotifyAsync(
                quiz.CourseId,
                $"{quiz.Course.Name}: New Quiz",
                $"\"{quiz.Title}\" is now available. Start solving now!",
                NotificationType.NewQuizAdded,
                $"https://www.ailern.me/quizzes/{quiz.Id}",
                "Start Quiz"
            );
        }

        return Result.Success("Quiz updated successfully");
    }
}
