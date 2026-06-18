using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Notifications.Commands.DeleteSingleNotification;

public record DeleteSingleNotificationCommand(Guid NotificationId) : IRequest<Result>;
