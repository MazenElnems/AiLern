using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Notifications.Commands.DeleteSingleNotification;

public class DeleteSingleNotificationCommandHandler : IRequestHandler<DeleteSingleNotificationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public DeleteSingleNotificationCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(DeleteSingleNotificationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var userNotification = await _unitOfWork.UserNotifications.Query
            .FirstOrDefaultAsync(n => n.UserId == userId && n.NotificationId == request.NotificationId, cancellationToken);

        if(userNotification == null)
            return DomainErrors.Common.NotFound(nameof(UserNotification), "UserNotification not found for the current user.");

        _unitOfWork.UserNotifications.Delete(userNotification);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success("UserNotification deleted successfully.");
    }
}
