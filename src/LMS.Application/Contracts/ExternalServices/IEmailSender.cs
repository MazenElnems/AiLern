namespace LMS.Application.Contracts.ExternalServices;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string email, string fullName ,string token);
    Task SendChangeEmailConfirmationAsync(int userId, string email, string fullName, string token);
    Task SendForgetPasswordEmailAsync(string email, string fullName, string token);
    Task SendWelcomeEmailAsync(string email, string fullName);
}
