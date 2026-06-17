using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Notifications.Commands.MarkAllAsRead;

public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public MarkAllAsReadCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var notifications = await _unitOfWork.UserNotifications.FilterAsync(n => n.UserId == userId && !n.IsRead);
        if (notifications == null || !notifications.Any())
        {
            return DomainErrors.Notifications.NotFound(Guid.Empty);
        }

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            _unitOfWork.UserNotifications.Update(notification);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
