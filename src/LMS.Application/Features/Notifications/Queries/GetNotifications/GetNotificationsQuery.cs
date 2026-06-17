using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Notifications.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
