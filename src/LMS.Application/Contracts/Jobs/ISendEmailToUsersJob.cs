using LMS.Domain.Models.Notification;

namespace LMS.Application.Contracts.Jobs;

public interface ISendEmailToUsersJob
{
    Task ExecuteAsync(IEnumerable<int> UsersId, NotificationEmailModel notificationEmailModel);
}
