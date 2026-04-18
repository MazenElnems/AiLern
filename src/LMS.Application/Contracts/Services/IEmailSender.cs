using LMS.Domain.Models.Notification;

namespace LMS.Application.Contracts.Services;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string email, string fullName ,string token);
    Task SendForgetPasswordEmailAsync(string email, string fullName, string token);
    Task SendWelcomeEmailAsync(string email, string fullName);

    Task SendNotificationEmailWithUrlAsync(string email, string fullName, NotificationEmailModel notificationEmailModel);
}
