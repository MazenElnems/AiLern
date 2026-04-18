using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly INotificationService _notificationService;

    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBackgroundJobService backgroundJobService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _backgroundJobService = backgroundJobService;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);
        if (quiz is null)
            return Result<Guid>.Failure(DomainErrors.Quiz.NotFound(request.QuizId));
        if (quiz.Course.InstructorId != userId)
            return Result<Guid>.Failure(DomainErrors.Quiz.NotOwned);

        if (quiz.AvailableUntil < DateTime.UtcNow)
            return DomainErrors.Quiz.QuizFinished;

        // Can't update quiz after start time
        if (DateTime.UtcNow > quiz.AvailableFrom)
            return DomainErrors.Quiz.UpdateNotAllowedAfterStart;

        if(quiz.Status == QuizStatus.Scheduled && request.Quiz.Status != QuizStatus.Scheduled)
        {

            if (!string.IsNullOrEmpty(quiz.PublishBackgroundJobId))
            {
                _backgroundJobService.Delete(quiz.PublishBackgroundJobId);
            }
        }

        var previousStatus = quiz.Status;

        quiz.Title = request.Quiz.Title;
        quiz.Description = request.Quiz.Description;
        quiz.AvailableFrom = request.Quiz.AvailableFrom;
        quiz.AvailableUntil = request.Quiz.AvailableUntil;
        quiz.ShowResultOnClose = request.Quiz.ShowResultOnClose;
        quiz.MaximumAttempts = request.Quiz.MaximumAttempts;
        quiz.ShuffleQuestions = request.Quiz.ShuffleQuestions;
        quiz.ShuffleOptions = request.Quiz.ShuffleOptions;
        quiz.Status = request.Quiz.Status;

        if (quiz.Status == QuizStatus.Published)
            quiz.PublishedAt = DateTime.UtcNow;

        else if (quiz.Status == QuizStatus.Draft)
            quiz.PublishedAt = null;

        await _unitOfWork.CommitAsync();

        if (quiz.Status == QuizStatus.Scheduled)
        {
            quiz.PublishedAt = null;
            quiz.PublishBackgroundJobId = _backgroundJobService.Schedule<IQuizPublishSchedulerJob>((job) => job.ExecuteAsync(quiz.Id), request.Quiz.PublishedDate!.Value);
            await _unitOfWork.CommitAsync();


        }


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

        return Result<Guid>.Success(quiz.Id, "quiz updated successfully.");
    }
}
