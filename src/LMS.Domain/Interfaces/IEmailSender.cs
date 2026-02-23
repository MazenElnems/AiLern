namespace LMS.Domain.Repositories;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string email, string fullName ,string token);
    Task SendForgetPasswordEmailAsync(string email, string fullName, string token);
    Task SendWelcomeEmailAsync(string email, string fullName);
}
