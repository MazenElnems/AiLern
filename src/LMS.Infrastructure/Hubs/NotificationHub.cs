using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Constants;
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
}
