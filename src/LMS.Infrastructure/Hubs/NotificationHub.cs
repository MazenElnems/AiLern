using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Notification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.Infrastructure.Hubs;  

public class NotificationHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User!.IsInRole(UserRoles.Student))
        {
            var userId = Context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var courseIds = await _unitOfWork.Enrollments.Query
                .Where(e => e.StudentId == int.Parse(userId))
                .Select(e => e.CourseId)
                .ToListAsync();

            foreach (var id in courseIds)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{id}");
        }
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task CourseMaterialAdded(int courseId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        var studentsToNotify = await _unitOfWork.Enrollments.Query
                .AsNoTracking()
                .Where(e => e.CourseId == course!.Id)
                .Select(e => e.StudentId)
                .ToListAsync();

        var type = NotificationType.CourseMaterialsUpdated;

        var notification = new Notification
        {
            Title = $"{course!.Name}: New Material",
            Message = $"New materials has been added/updated in \"{course.Name}\" course",
            CreatedAt = DateTime.UtcNow,
            Type = type,
            Url = $"/courses/{course.Id}/sections"
        };

        await _unitOfWork.Notfications.InsertAsync(notification);

        var userNotifications = studentsToNotify.Select(s => new UserNotification
        {
            NotificationId = notification.NotificationId,
            UserId = s,
            IsRead = false
        });

        await _unitOfWork.UserNotifications.InsertRangeAsync(userNotifications);

        await _unitOfWork.CommitAsync();

        await Clients.Group($"course-{course.Id}").SendAsync("recieveNotification",notification.Title,notification.Message,type);
    }
}
