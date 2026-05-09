using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Models.Notification;
using LMS.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LMS.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundService;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork, IBackgroundJobService backgroundService)
    {
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
        _backgroundService = backgroundService;
    }

    public async Task NotifyAsync(int courseId, string title, string message,NotificationType type,string url,string? actionText)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        var group = $"course-{courseId}";
        var studentsToNotify = await _unitOfWork.Enrollments.Query.AsNoTracking()
                .Where(e => e.CourseId == courseId).Select(e => e.StudentId).ToListAsync();

        var notification = new Notification
        {
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            Type = type,
            Url = url
        };
        await _unitOfWork.Notfications.InsertAsync(notification);

        var userNotifications = studentsToNotify.Select(s => new UserNotification
        {
            NotificationId = notification.NotificationId,
            UserId = s,
            IsRead = false
        });

        await _unitOfWork.UserNotifications.InsertRangeAsync(userNotifications);

        var notificationEmailModel = new NotificationEmailModel
        {
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = url,
            ActionText = actionText
        };

        _backgroundService.Enqueue<ISendEmailToUsersJob>((job) => job.ExecuteAsync(studentsToNotify, notificationEmailModel));
        await _hubContext.Clients.Group(group).SendAsync("recieveNotification", title, message);

        await _unitOfWork.CommitAsync();
    }
    

    public async Task NotifyAsync(int userId, string title, string message)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("recieveNotification", title, message);
    }

    public async Task NotifyUserWithEmailAsync(int userId, string title, string message, NotificationType type, string url, string? actionText)
    {
        var notification = new Notification
        {
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            Type = type,
            Url = url
        };
        await _unitOfWork.Notfications.InsertAsync(notification);

        var userNotifications = new UserNotification
        {
            NotificationId = notification.NotificationId,
            UserId = userId,
            IsRead = false
        };

        await _unitOfWork.UserNotifications.InsertAsync(userNotifications);

        var notificationEmailModel = new NotificationEmailModel
        {
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = url,
            ActionText = actionText
        };

        _backgroundService.Enqueue<ISendEmailToUsersJob>((job) => job.ExecuteAsync(new[]{userId}, notificationEmailModel));
        await _hubContext.Clients.User(userId.ToString()).SendAsync("recieveNotification", title, message);
        await _unitOfWork.CommitAsync();
    }
}
