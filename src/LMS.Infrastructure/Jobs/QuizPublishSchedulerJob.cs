using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Enums;

namespace LMS.Infrastructure.Jobs;

internal class QuizPublishSchedulerJob : IQuizPublishSchedulerJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public QuizPublishSchedulerJob(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task ExecuteAsync(Guid quizId)
    {
        var quiz = await _unitOfWork.Quizzes.GetByIdAsync(quizId);

        if (quiz == null)
            return;

        if(quiz.Status == QuizStatus.Scheduled)
        {
            quiz.Status = QuizStatus.Published;
            quiz.PublishedAt = DateTime.UtcNow;
            var course = await _unitOfWork.Courses.GetByIdAsync(quiz.CourseId);
            await _unitOfWork.CommitAsync();

            await _notificationService.NotifyAsync(
                quiz.CourseId,
                $"{course!.Name}: New Quiz",
                $"\"{quiz.Title}\" is now available. Start solving now!",
                NotificationType.NewQuizAdded,
                $"https://www.ailern.me/quizzes/{quiz.Id}",
                "Start Quiz"
            );
        }
    }
}
