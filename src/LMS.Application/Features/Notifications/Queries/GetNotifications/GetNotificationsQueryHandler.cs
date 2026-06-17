using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Notifications.Queries.GetNotifications;
using LMS.Application.Features.Notifications.Shared.DTO;
using LMS.Domain.Entities.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Notifications.Queries.GetLastThreeNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _user;

        public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, IUserContext user)
        {
            _unitOfWork = unitOfWork;
            _user = user;
        }

        public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _user.GetCurrentUser().Id;
            var notifications = await _unitOfWork.UserNotifications.Query.AsNoTracking()
                .Where(n => n.UserId == userId)
                .Include(n => n.Notification)
                .OrderByDescending(n => n.Notification.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var notificationDtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Notification.NotificationId,
                Title = n.Notification.Title,
                Message = n.Notification.Message,
                CreatedAt = n.Notification.CreatedAt,
                Type = n.Notification.Type,
                Url = n.Notification.Url,
                IsRead = n.IsRead
            }).ToList();

            return Result<List<NotificationDto>>.Success(notificationDtos);
        }
    }
}
